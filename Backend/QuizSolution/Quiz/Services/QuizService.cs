using AutoMapper;
using QuizApi.Dtos.Quiz;
using QuizApi.Exceptions.Quiz;
using QuizApi.Exceptions.User;
using QuizApi.Interfaces.Repository;
using QuizApi.Interfaces.Service;
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

        public QuizService(IQuizRepository<int, Quiz> quizRepository, IUserRepository<int, User> userRepository, IMapper mapper, ILogger<QuizService> logger)
        {
            _quizRepository = quizRepository;
            _userRepository = userRepository;
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

         
            quiz.CreatedAt = DateTime.UtcNow;
            quiz.Code = GenerateUniqueCode();

            

            quiz.Questions = createQuizDTO.Questions.Select(q => new Question
            {
                QuestionText = q.QuestionText,
                Options = q.Options.Select(o => new Option { OptionText = o, IsAnswer=q.CorrectAnswers.Contains(o)}).ToList(),             
            }).ToList();

           /* if (createQuizDTO.Tags != null && createQuizDTO.Tags.Any())
            {
                quiz.QuizTags = createQuizDTO.Tags.Select(tag => new QuizTag
                {
                    TagEntity = new Tag { TagName = tag }
                }).ToList();
            }*/
            
            if (createQuizDTO.Type == "private")
            {
                quiz.AllowedUsers = createQuizDTO.AllowedUsers.Select(email => new AllowedUser
                {
                    User = _userRepository.GetUserByEmail(email).Result // Assuming GetUserByEmail is synchronous
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
            
            if (quiz.Type == "private")
            {                
                if (quiz.AllowedUsers.FirstOrDefault(a => a.UserId == user.UserId)==null)
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

        public async Task<bool> CompleteQuiz(CompleteQuizDTO completeQuizDTO)
        {
            var quiz = await _quizRepository.GetQuizById(completeQuizDTO.QuizId);
            if (quiz == null)
            {
                throw new QuizNotFoundException("Quiz not found");
            }

            // Further logic for calculating score, awarding certificates, etc.

            return true;
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
