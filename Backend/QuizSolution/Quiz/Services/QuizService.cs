using AutoMapper;
using QuizApi.Dtos.Quiz;
using QuizApi.Exceptions.Quiz;
using QuizApi.Exceptions.User;
using QuizApi.Interfaces.Repository;
using QuizApi.Interfaces.Service;
using QuizApi.Repositories;
using QuizApp.Models;

namespace QuizApi.Services
{
    public class QuizService
  : IQuizService
    {
        private readonly IQuizRepository<int, Quiz> _quizRepository;
        private readonly IUserRepository<int, User> _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<QuizService> _logger;
        private readonly ICertificateRepository<int, Certificate> _certificateRepository;
        private readonly ITagRepository<int, Tag> _tagRepository;
        private readonly IAttemptRepository<int, Attempt> _attemptRepository;
        private readonly IQuestionRepository<int, Question> _questionRepository;
        private readonly IAllowedUserRepository<int, AllowedUser> _allowedUserRepository;

        public QuizService(IQuizRepository<int, Quiz> quizRepository,ITagRepository<int , Tag> tagRepository, IUserRepository<int, User> userRepository, IMapper mapper, ILogger<QuizService> logger, IAllowedUserRepository<int, AllowedUser> allowedUserRepository, IQuestionRepository<int, Question> questionRepository, IAttemptRepository<int, Attempt> attemptRepository, ICertificateRepository<int, Certificate> certificateRepository)
        {
            _quizRepository = quizRepository;
            _certificateRepository = certificateRepository;
            _attemptRepository = attemptRepository;
            _questionRepository = questionRepository;
            _userRepository = userRepository;
            _tagRepository = tagRepository;
            _allowedUserRepository = allowedUserRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<QuizDTO>> GetQuizzes(string topic = null, List<string> tags = null)
        {
            var quizzes = await _quizRepository.GetQuizzesByTopicAndTags(topic, tags);
            return _mapper.Map<IEnumerable<QuizDTO>>(quizzes);
        }

        public async Task<QuizDTO> CreateQuiz(CreateQuizDTO createQuizDTO)
        {
            
            var quiz = MapCreateQuizDTOtoQuiz(createQuizDTO);
            var allTags = await _tagRepository.GetAllTag();


            quiz.CreatedAt = DateTime.UtcNow;
            quiz.Code = GenerateUniqueCode();

            

            quiz.Questions = createQuizDTO.Questions.Select(q => new Question
            {
                QuestionText = q.QuestionText,
                Options = q.Options.Select(o => new Option { OptionText = o, IsAnswer=q.CorrectAnswers.Contains(o)}).ToList(),             
            }).ToList();

   /*         if (createQuizDTO.Tags != null && createQuizDTO.Tags.Any())
            {

                quiz.QuizTags = createQuizDTO.Tags.Find(tag => tag.C(allTags.Any());
                quiz.QuizTags = createQuizDTO.Tags.(tag => new QuizTag
                {
                    Tag = tag.con
                }).ToList();
            }*/

            if (createQuizDTO.Type == "private")
            {
                quiz.AllowedUsers = createQuizDTO.AllowedUsers.Select(email => new AllowedUser
                {
                    User = _userRepository.GetUserByEmail(email).Result 
                }).ToList();
            }

          
            quiz = await _quizRepository.AddQuiz(quiz);
            
            var quizDTO = _mapper.Map<QuizDTO>(quiz);

            return quizDTO;
        }

        private Quiz MapCreateQuizDTOtoQuiz(CreateQuizDTO createQuizDTO)
        {
            var quiz = new Quiz() {CreatorId = createQuizDTO.UserId, Topic = createQuizDTO.Topic, Description = createQuizDTO.Description, Duration = createQuizDTO.Duration, StartTime = createQuizDTO.StartTime, EndTime = createQuizDTO.EndTime, Music = createQuizDTO.Music, Type = createQuizDTO.Type };
            return quiz;
        }

        public async Task<ReturnAttendQuizDTO> AttendQuiz(AttendQuizDTO attendQuizDTO)
        {
            var quiz = await _quizRepository.GetQuizById(attendQuizDTO.QuizId);
            var user = await _userRepository.GetUserByEmail(attendQuizDTO.Email);
            if (user == null)
                throw new UserNotFoundException("User not Found");
            if (quiz == null)
                throw new QuizNotFoundException("Quiz Not Found");


            var isCodeValid = quiz.Code == attendQuizDTO.Code;
            if (!isCodeValid)
            {
                throw new InvalidQuizCodeException("Invalid Quiz Code");
            }

            if (!(quiz.StartTime < DateTime.UtcNow & quiz.EndTime > DateTime.UtcNow))
                throw new QuizTimeException("Quiz Time Not Allowed");


            if (quiz.Type == "private")
            {
                var type = (await _allowedUserRepository.ValidateQuizAccess(user.UserId, quiz.QuizId));                
                if (type.Count() == 0)
                {
                    throw new PrivateQuizException("User not allowed to attend this quiz");
                }
            }            
            return  new ReturnAttendQuizDTO()
            {
                QuizId = quiz.QuizId,
                Emailid = attendQuizDTO.Email,
                StartTime = quiz.StartTime,
                EndTime = quiz.EndTime,
                Duration = quiz.Duration,
                QuestionDto = await GetQuizQuestions(quiz.QuizId)
            };
            
        }

       public async Task<ReturnCompleteQuizDTO> CompleteQuiz(CompleteQuizDTO completeQuizDTO)
        {
            // Check if user exists
            var user = await _userRepository.GetUserByEmail(completeQuizDTO.EmailId);
            if(user == null)
            {
                throw new UserNotFoundException("User not found");
            }
            // Check if quiz exists
            var quiz = await _quizRepository.GetQuizById(completeQuizDTO.QuizId);
            if (quiz == null)
            {
                throw new QuizNotFoundException("Quiz not found");
            }

            // Calculate the time taken to complete the quiz
            var timeTakenToComplete = completeQuizDTO.EndTime - completeQuizDTO.StartTime;
            if (timeTakenToComplete.TotalMinutes > quiz.Duration)
            {
                throw new TimeLimitExceededException("Time limit exceeded");
            }          

            // Retrieve questions and correct answers from the database
            var questions = await _quizRepository.GetQuestionsByQuizId(completeQuizDTO.QuizId);
            int totalQuestions = questions.Count();
            int correctAnswersCount = 0;

            // Calculate the score
            foreach (var userAnswer in completeQuizDTO.Answers)
            {
                var question = questions.FirstOrDefault(q => q.QuestionId == userAnswer.QuestionId);
                if (question != null)
                {
                    var correctOptions = question.Options.Where(co => co.IsAnswer == true).Select(co=>co.OptionText);                    
                    if (!correctOptions.Except(userAnswer.SelectedAnswers).Any() && !userAnswer.SelectedAnswers.Except(correctOptions).Any())
                    {
                        correctAnswersCount++;
                    }
                }
            }
            // Calculate percentage
            double scorePercentage = (correctAnswersCount / (double)totalQuestions) * 100;

            
            var attempt = new Attempt
            {
                UserId = user.UserId,
                QuizId = completeQuizDTO.QuizId,
                Score = (int)scorePercentage,
                CompletedAt = DateTime.UtcNow
            };
            await _attemptRepository.AddAttempt(attempt);

            var status = "fail";
            // Award certificates based on the score and time taken
            if (scorePercentage >= 80)
            {

                status = "pass";
                // Check if the special certificate conditions are met
                if (timeTakenToComplete.TotalMinutes <= (quiz.Duration / 2))
                {
                    var  spccertificate = new Certificate
                    {
                        AttemptId = attempt.AttemptId,
                        UserId = user.UserId,
                        QuizId = completeQuizDTO.QuizId,
                        Url = "Special",
                        CertType = "Special",
                        // Url = GenerateSpecialCertificateUrl(attempt.AttemptId)
                    };
                    await _certificateRepository.AddCertificate(spccertificate);
                }
                var certificate = new Certificate
                {
                    AttemptId = attempt.AttemptId,
                    UserId = user.UserId,
                    QuizId = completeQuizDTO.QuizId,
                    CertType = "Normal",
                    Url = "Normal"//GenerateCertificateUrl(attempt.AttemptId)
                };

                await _certificateRepository.AddCertificate(certificate);
            }            
            return new ReturnCompleteQuizDTO() { QuizTopic = quiz.Topic,Score = scorePercentage, CertUrl = "certurl",Status = status};
        }
       

        private string GenerateUniqueCode()
        {
            return new Random().Next(100000, 999999).ToString();
        }

        private async Task<IEnumerable<QuestionDTO>> GetQuizQuestions(int quizId)
        {
            var questions = await _quizRepository.GetQuestionsByQuizId(quizId);
            return _mapper.Map<IEnumerable<QuestionDTO>>(questions);
        }

       
    }
}
