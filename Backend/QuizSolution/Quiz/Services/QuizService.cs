using AutoMapper;
using QuizApi.Dtos.Quiz;
using QuizApi.Exceptions.Quiz;
using QuizApi.Exceptions.User;
using QuizApi.Interfaces.Repository;
using QuizApi.Interfaces.Service;
using QuizApi.Repositories;
using QuizApp.Models;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

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
        private readonly IQuizTagRepository<string, QuizTag> _quizTagRepository;
        private readonly IAllowedUserRepository<int, AllowedUser> _allowedUserRepository;
        private readonly IDistributedCache _cache;

        public QuizService(IQuizRepository<int, Quiz> quizRepository, IDistributedCache cache, ITagRepository<int, Tag> tagRepository, IUserRepository<int, User> userRepository, IMapper mapper, ILogger<QuizService> logger, IAllowedUserRepository<int, AllowedUser> allowedUserRepository, IQuestionRepository<int, Question> questionRepository, IAttemptRepository<int, Attempt> attemptRepository, ICertificateRepository<int, Certificate> certificateRepository, IQuizTagRepository<string, QuizTag> quizTagRepository)
        {
            _cache = cache;
            _quizRepository = quizRepository;
            _quizTagRepository = quizTagRepository;
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
            var cacheKey = $"quizzes_{topic}";
            var cachedQuizzes = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedQuizzes))
            {
                return JsonSerializer.Deserialize<IEnumerable<QuizDTO>>(cachedQuizzes);
            }
            var quizzes = await _quizRepository.GetQuizzesByTopicAndTags(topic, tags);
            var quizDTOs = _mapper.Map<IEnumerable<QuizDTO>>(quizzes);
            var cacheOptions = new DistributedCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(5))
            .SetAbsoluteExpiration(TimeSpan.FromHours(1));

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(quizDTOs), cacheOptions);

            return quizDTOs;
        }

        public async Task<QuizDTO> CreateQuiz(CreateQuizDTO createQuizDTO)
        {

            var quiz = MapCreateQuizDTOtoQuiz(createQuizDTO);

            quiz.CreatedAt = DateTime.UtcNow;
            quiz.Code = GenerateUniqueCode();



            quiz.Questions = createQuizDTO.Questions.Select(q => new Question
            {
                QuestionText = q.QuestionText,
                Options = q.Options.Select(o => new Option { OptionText = o, IsAnswer = q.CorrectAnswers.Contains(o) }).ToList(),
            }).ToList();


            var inviteUserToQuiz = new List<string>();
            var inviteUserToApp = new List<string>();
            if (createQuizDTO.Type == "private")
            {


                foreach (var email in createQuizDTO.AllowedUsers)
                {

                    var existingUser = await _userRepository.GetUserByEmail(email);

                    if (existingUser == null)
                    {
                        inviteUserToApp.Add(email);
                        var newUser = new User() { Email = email, Name = email, Password = "pass@123" };
                        await _userRepository.AddUser(newUser);


                    }
                    else
                    {
                        inviteUserToQuiz.Add(email);
                    }
                }


                quiz.AllowedUsers = createQuizDTO.AllowedUsers.Select(email => new AllowedUser
                {
                    User = _userRepository.GetUserByEmail(email).Result
                }).ToList();
            }


            quiz = await _quizRepository.AddQuiz(quiz);

            var quizDTO = _mapper.Map<QuizDTO>(quiz);
            var baseEmailURI = Environment.GetEnvironmentVariable("BASE_URL_SEND_INVITATION");
            if (baseEmailURI.Length > 0)
            {
                if (inviteUserToApp.Count > 0)
                    SendInvitationToNewUsers(new SendInviteDTO() { quizId = quiz.QuizId, recipients = inviteUserToApp, quizCode = quiz.Code, usertype = "new" });
                if (inviteUserToQuiz.Count > 0)
                    SendInvitationToNewUsers(new SendInviteDTO() { quizId = quiz.QuizId, recipients = inviteUserToQuiz, quizCode = quiz.Code, usertype = "old" });
            }
            return quizDTO;
        }
        private SendInviteReturnDTO SendInvitationToNewUsers(SendInviteDTO sendInviteDTO)
        {
            var client = new HttpClient();
            var baseEmailURI = Environment.GetEnvironmentVariable("BASE_URL_SEND_INVITATION");
            var inviteURL = Environment.GetEnvironmentVariable("SEND_INVITATION_URL");
            var json = JsonSerializer.Serialize(sendInviteDTO);
            client.BaseAddress = new Uri(baseEmailURI);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = client.PostAsync(inviteURL, content).Result;
            if (response.IsSuccessStatusCode)
            {
                var responseContext = response.Content.ReadAsStringAsync().Result;
                var certResponse = JsonSerializer.Deserialize<SendInviteReturnDTO>(responseContext);
                return certResponse;
            }
            else
            {
                throw new InviteNotSendException(response.Content.ReadAsStringAsync().Result);
            }
        }
        private Quiz MapCreateQuizDTOtoQuiz(CreateQuizDTO createQuizDTO)
        {
            var quiz = new Quiz() { CreatorId = createQuizDTO.UserId, ImageURL = createQuizDTO.ImageURL, Topic = createQuizDTO.Topic, Description = createQuizDTO.Description, Duration = createQuizDTO.Duration, StartTime = createQuizDTO.StartTime, EndTime = createQuizDTO.EndTime, Type = createQuizDTO.Type, DurationPerQuestion = createQuizDTO.DurationPerQuestion };
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

            if (!(quiz.StartTime < DateTime.Now & quiz.EndTime > DateTime.Now))
                throw new QuizTimeException("Quiz Time Not Allowed");


            if (quiz.Type == "private")
            {
                var type = (await _allowedUserRepository.ValidateQuizAccess(user.UserId, quiz.QuizId));
                if (type.Count() == 0)
                {
                    throw new PrivateQuizException("User not allowed to attend this quiz");
                }
            }
            return new ReturnAttendQuizDTO()
            {
                QuizId = quiz.QuizId,
                Emailid = attendQuizDTO.Email,
                StartTime = quiz.StartTime,
                EndTime = quiz.EndTime,
                Duration = quiz.Duration,
                QuestionDto = await GetQuizQuestions(quiz.QuizId),
                DurationPerQuestion = quiz.DurationPerQuestion,
            };

        }

        public async Task<ReturnCompleteQuizDTO> CompleteQuiz(CompleteQuizDTO completeQuizDTO)
        {
            // Check if user exists
            var user = await _userRepository.GetUserByEmail(completeQuizDTO.EmailId);
            if (user == null)
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
                    var correctOptions = question.Options.Where(co => co.IsAnswer == true).Select(co => co.OptionText);
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
                CompletedAt = DateTime.UtcNow,
            };
            await _attemptRepository.AddAttempt(attempt);


            var status = "fail";
            var certUrl = "";
            var isNormal = true;
            var baseurl = Environment.GetEnvironmentVariable("BASE_URL_GENERATE_CERTIFICATE");
            var urlAvailable = false;
            if (baseurl.Length > 0) urlAvailable = true;

            // Award certificates based on the score and time taken
            if (scorePercentage >= 80)
            {

                status = "pass";
                // Check if the special certificate conditions are met
                if (timeTakenToComplete.TotalMinutes <= (quiz.Duration / 2))
                {
                    isNormal = false;
                }


                var certDetails = urlAvailable ? GenerateCertificate(new GenerateCertDTO() { name = user.Name, score = scorePercentage.ToString(), quizTopic = quiz.Topic, certType = isNormal ? "Normal" : "Special" }) : null;

                var certificate = new Certificate
                {
                    AttemptId = attempt.AttemptId,
                    UserId = user.UserId,
                    QuizId = completeQuizDTO.QuizId,
                    Url = urlAvailable ? certDetails.pdfUrl : "",
                    CertType = isNormal ? "Normal" : "Special",
                };
                certUrl = urlAvailable ? certDetails.pdfUrl : "";
                await _certificateRepository.AddCertificate(certificate);
            }
            return new ReturnCompleteQuizDTO() { QuizTopic = quiz.Topic, Score = scorePercentage, CertUrl = certUrl, Status = status };
        }

        private GenerateCertReturnDTO GenerateCertificate(GenerateCertDTO cert)
        {
            var client = new HttpClient();
            var baseurl = Environment.GetEnvironmentVariable("BASE_URL_GENERATE_CERTIFICATE");
            var url = Environment.GetEnvironmentVariable("GENERATE_CERTIFICATE_URL");
            client.BaseAddress = new Uri(baseurl);
            var json = JsonSerializer.Serialize(cert);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                var responseContext = response.Content.ReadAsStringAsync().Result;
                var certResponse = JsonSerializer.Deserialize<GenerateCertReturnDTO>(responseContext);
                return certResponse;
            }
            else
            {
                throw new CertificateNotGenerated(response.Content.ReadAsStringAsync().Result);
            }
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
