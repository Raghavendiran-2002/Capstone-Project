using AutoMapper;
using Moq;
using NUnit.Framework;
using QuizApi.Dtos.Quiz;
using QuizApi.Exceptions.Quiz;
using QuizApi.Exceptions.User;
using QuizApi.Interfaces.Repository;
using QuizApi.Interfaces.Service;
using QuizApi.Models;
using QuizApi.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;

namespace QuizApi.Tests.Services
{
    [TestFixture]
    public class QuizServiceTests
    {
        private Mock<IQuizRepository<int, Quiz>> _quizRepositoryMock;
        private Mock<IUserRepository<int, User>> _userRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ILogger<QuizService>> _loggerMock;
        private Mock<ICertificateRepository<int, Certificate>> _certificateRepositoryMock;
        private Mock<ITagRepository<int, Tag>> _tagRepositoryMock;
        private Mock<IAttemptRepository<int, Attempt>> _attemptRepositoryMock;
        private Mock<IQuestionRepository<int, Question>> _questionRepositoryMock;
        private Mock<IQuizTagRepository<string, QuizTag>> _quizTagRepositoryMock;
        private Mock<IAllowedUserRepository<int, AllowedUser>> _allowedUserRepositoryMock;
        private Mock<IDistributedCache> _cacheMock;

        private IQuizService _quizService;

        [SetUp]
        public void SetUp()
        {
            _quizRepositoryMock = new Mock<IQuizRepository<int, Quiz>>();
            _userRepositoryMock = new Mock<IUserRepository<int, User>>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<QuizService>>();
            _certificateRepositoryMock = new Mock<ICertificateRepository<int, Certificate>>();
            _tagRepositoryMock = new Mock<ITagRepository<int, Tag>>();
            _attemptRepositoryMock = new Mock<IAttemptRepository<int, Attempt>>();
            _questionRepositoryMock = new Mock<IQuestionRepository<int, Question>>();
            _quizTagRepositoryMock = new Mock<IQuizTagRepository<string, QuizTag>>();
            _allowedUserRepositoryMock = new Mock<IAllowedUserRepository<int, AllowedUser>>();
            _cacheMock = new Mock<IDistributedCache>();

            _quizService = new QuizService(
                _quizRepositoryMock.Object,
                _cacheMock.Object,
                _tagRepositoryMock.Object,
                _userRepositoryMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _allowedUserRepositoryMock.Object,
                _questionRepositoryMock.Object,
                _attemptRepositoryMock.Object,
                _certificateRepositoryMock.Object,
                _quizTagRepositoryMock.Object);
        }

        [Test]
        public async Task GetQuizzes_ReturnsQuizzesFromCache_WhenCacheIsAvailable()
        {
            // Arrange
            var topic = "science";
            var cacheKey = $"quizzes_{topic}";
            var cachedQuizzes = new List<QuizDTO> { new QuizDTO { QuizId = 1, Topic = topic } };
            var cachedQuizzesString = JsonSerializer.Serialize(cachedQuizzes);

            _cacheMock.Setup(c => c.GetStringAsync(cacheKey)).ReturnsAsync(cachedQuizzesString);

            // Act
            var result = await _quizService.GetQuizzes(topic);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(topic, result.First().Topic);

            _cacheMock.Verify(c => c.GetStringAsync(cacheKey), Times.Once);
        }

        [Test]
        public async Task GetQuizzes_ReturnsQuizzesFromRepository_WhenCacheIsNotAvailable()
        {
            // Arrange
            var topic = "science";
            var cacheKey = $"quizzes_{topic}";
            var quizzes = new List<Quiz> { new Quiz { QuizId = 1, Topic = topic } };
            var quizzesDto = new List<QuizDTO> { new QuizDTO { QuizId = 1, Topic = topic } };

            _cacheMock.Setup(c => c.GetStringAsync(cacheKey)).ReturnsAsync((string)null);
            _quizRepositoryMock.Setup(r => r.GetQuizzesByTopicAndTags(topic, null)).ReturnsAsync(quizzes);
            _mapperMock.Setup(m => m.Map<IEnumerable<QuizDTO>>(quizzes)).Returns(quizzesDto);

            // Act
            var result = await _quizService.GetQuizzes(topic);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(topic, result.First().Topic);

            _cacheMock.Verify(c => c.GetStringAsync(cacheKey), Times.Once);
            _quizRepositoryMock.Verify(r => r.GetQuizzesByTopicAndTags(topic, null), Times.Once);
            _mapperMock.Verify(m => m.Map<IEnumerable<QuizDTO>>(quizzes), Times.Once);
        }

        [Test]
        public async Task GetQuizzes_SetsCache_WhenCacheIsNotAvailable()
        {
            // Arrange
            var topic = "science";
            var cacheKey = $"quizzes_{topic}";
            var quizzes = new List<Quiz> { new Quiz { QuizId = 1, Topic = topic } };
            var quizzesDto = new List<QuizDTO> { new QuizDTO { QuizId = 1, Topic = topic } };

            _cacheMock.Setup(c => c.GetStringAsync(cacheKey)).ReturnsAsync((string)null);
            _quizRepositoryMock.Setup(r => r.GetQuizzesByTopicAndTags(topic, null)).ReturnsAsync(quizzes);
            _mapperMock.Setup(m => m.Map<IEnumerable<QuizDTO>>(quizzes)).Returns(quizzesDto);

            // Act
            var result = await _quizService.GetQuizzes(topic);

            // Assert
            _cacheMock.Verify(c => c.SetStringAsync(cacheKey, It.IsAny<string>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
        }

        [Test]
        public async Task CreateQuiz_ThrowsException_WhenQuizCodeIsInvalid()
        {
            // Arrange
            var createQuizDTO = new CreateQuizDTO { UserId = 1, AllowedUsers = new List<string> { "test@example.com" }, Questions = new List<CreateQuestionDTO>(), Type = "private" };

            _userRepositoryMock.Setup(r => r.GetUserByEmail(It.IsAny<string>())).ReturnsAsync((User)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidQuizCodeException>(() => _quizService.CreateQuiz(createQuizDTO));
            Assert.AreEqual("Invalid Quiz Code", ex.Message);
        }

        [Test]
        public async Task AttendQuiz_ThrowsUserNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            var attendQuizDTO = new AttendQuizDTO { Email = "nonexistent@example.com", QuizId = 1, Code = "123456" };
            _userRepositoryMock.Setup(r => r.GetUserByEmail(attendQuizDTO.Email)).ReturnsAsync((User)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<UserNotFoundException>(() => _quizService.AttendQuiz(attendQuizDTO));
            Assert.AreEqual("User not Found", ex.Message);
        }

        [Test]
        public async Task AttendQuiz_ThrowsQuizNotFoundException_WhenQuizDoesNotExist()
        {
            // Arrange
            var attendQuizDTO = new AttendQuizDTO { Email = "test@example.com", QuizId = 1, Code = "123456" };
            var user = new User { Email = "test@example.com", UserId = 1 };
            _userRepositoryMock.Setup(r => r.GetUserByEmail(attendQuizDTO.Email)).ReturnsAsync(user);
            _quizRepositoryMock.Setup(r => r.GetQuizById(attendQuizDTO.QuizId)).ReturnsAsync((Quiz)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<QuizNotFoundException>(() => _quizService.AttendQuiz(attendQuizDTO));
            Assert.AreEqual("Quiz Not Found", ex.Message);
        }

        [Test]
        public async Task AttendQuiz_ThrowsInvalidQuizCodeException_WhenCodeIsInvalid()
        {
            // Arrange
            var attendQuizDTO = new AttendQuizDTO { Email = "test@example.com", QuizId = 1, Code = "123456" };
            var user = new User { Email = "test@example.com", UserId = 1 };
            var quiz = new Quiz { QuizId = 1, Code = "654321" };

            _userRepositoryMock.Setup(r => r.GetUserByEmail(attendQuizDTO.Email)).ReturnsAsync(user);
            _quizRepositoryMock.Setup(r => r.GetQuizById(attendQuizDTO.QuizId)).ReturnsAsync(quiz);

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidQuizCodeException>(() => _quizService.AttendQuiz(attendQuizDTO));
            Assert.AreEqual("Invalid Quiz Code", ex.Message);
        }

        [Test]
        public async Task AttendQuiz_ThrowsQuizTimeException_WhenQuizTimeIsInvalid()
        {
            // Arrange
            var attendQuizDTO = new AttendQuizDTO { Email = "test@example.com", QuizId = 1, Code = "123456" };
            var user = new User { Email = "test@example.com", UserId = 1 };
            var quiz = new Quiz { QuizId = 1, Code = "123456", StartTime = DateTime.Now.AddHours(1), EndTime = DateTime.Now.AddHours(2) };

            _userRepositoryMock.Setup(r => r.GetUserByEmail(attendQuizDTO.Email)).ReturnsAsync(user);
            _quizRepositoryMock.Setup(r => r.GetQuizById(attendQuizDTO.QuizId)).ReturnsAsync(quiz);

            // Act & Assert
            var ex = Assert.ThrowsAsync<QuizTimeException>(() => _quizService.AttendQuiz(attendQuizDTO));
            Assert.AreEqual("Quiz Time is Invalid", ex.Message);
        }

        [Test]
        public async Task AttendQuiz_ThrowsAllowedUserException_WhenUserNotAllowed()
        {
            // Arrange
            var attendQuizDTO = new AttendQuizDTO { Email = "test@example.com", QuizId = 1, Code = "123456" };
            var user = new User { Email = "test@example.com", UserId = 1 };
            var quiz = new Quiz { QuizId = 1, Code = "123456", Type = "private" };
            var allowedUser = new AllowedUser { QuizId = 1, UserId = 2 };

            _userRepositoryMock.Setup(r => r.GetUserByEmail(attendQuizDTO.Email)).ReturnsAsync(user);
            _quizRepositoryMock.Setup(r => r.GetQuizById(attendQuizDTO.QuizId)).ReturnsAsync(quiz);
            _allowedUserRepositoryMock.Setup(r => r.GetAllowedUser(attendQuizDTO.QuizId, user.UserId)).ReturnsAsync((AllowedUser)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<AllowedUserException>(() => _quizService.AttendQuiz(attendQuizDTO));
            Assert.AreEqual("User not Allowed", ex.Message);
        }

        [Test]
        public async Task AttendQuiz_ReturnsQuizDetails_WhenAllConditionsMet()
        {
            // Arrange
            var attendQuizDTO = new AttendQuizDTO { Email = "test@example.com", QuizId = 1, Code = "123456" };
            var user = new User { Email = "test@example.com", UserId = 1 };
            var quiz = new Quiz { QuizId = 1, Code = "123456", Type = "public" };
            var quizDTO = new QuizDTO { QuizId = 1, Topic = "science" };

            _userRepositoryMock.Setup(r => r.GetUserByEmail(attendQuizDTO.Email)).ReturnsAsync(user);
            _quizRepositoryMock.Setup(r => r.GetQuizById(attendQuizDTO.QuizId)).ReturnsAsync(quiz);
            _mapperMock.Setup(m => m.Map<QuizDTO>(quiz)).Returns(quizDTO);

            // Act
            var result = await _quizService.AttendQuiz(attendQuizDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(quizDTO.QuizId, result.QuizId);
            Assert.AreEqual(quizDTO.Topic, result.Topic);
        }

        [Test]
        public async Task CompleteQuiz_ThrowsQuizNotFoundException_WhenQuizDoesNotExist()
        {
            // Arrange
            var completeQuizDTO = new CompleteQuizDTO { UserId = 1, QuizId = 1, Answers = new List<AnswerDTO>() };
            _quizRepositoryMock.Setup(r => r.GetQuizById(completeQuizDTO.QuizId)).ReturnsAsync((Quiz)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<QuizNotFoundException>(() => _quizService.CompleteQuiz(completeQuizDTO));
            Assert.AreEqual("Quiz Not Found", ex.Message);
        }

        [Test]
        public async Task CompleteQuiz_ThrowsUserNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            var completeQuizDTO = new CompleteQuizDTO { UserId = 1, QuizId = 1, Answers = new List<AnswerDTO>() };
            var quiz = new Quiz { QuizId = 1 };

            _quizRepositoryMock.Setup(r => r.GetQuizById(completeQuizDTO.QuizId)).ReturnsAsync(quiz);
            _userRepositoryMock.Setup(r => r.GetUserById(completeQuizDTO.UserId)).ReturnsAsync((User)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<UserNotFoundException>(() => _quizService.CompleteQuiz(completeQuizDTO));
            Assert.AreEqual("User not Found", ex.Message);
        }

        [Test]
        public async Task CompleteQuiz_ThrowsQuizTimeException_WhenQuizHasEnded()
        {
            // Arrange
            var completeQuizDTO = new CompleteQuizDTO { UserId = 1, QuizId = 1, Answers = new List<AnswerDTO>() };
            var quiz = new Quiz { QuizId = 1, EndTime = DateTime.Now.AddMinutes(-10) };

            _quizRepositoryMock.Setup(r => r.GetQuizById(completeQuizDTO.QuizId)).ReturnsAsync(quiz);
            _userRepositoryMock.Setup(r => r.GetUserById(completeQuizDTO.UserId)).ReturnsAsync(new User());

            // Act & Assert
            var ex = Assert.ThrowsAsync<QuizTimeException>(() => _quizService.CompleteQuiz(completeQuizDTO));
            Assert.AreEqual("Quiz has Ended", ex.Message);
        }

        [Test]
        public async Task CompleteQuiz_ThrowsUserAttemptException_WhenUserHasAlreadyCompletedQuiz()
        {
            // Arrange
            var completeQuizDTO = new CompleteQuizDTO { UserId = 1, QuizId = 1, Answers = new List<AnswerDTO>() };
            var quiz = new Quiz { QuizId = 1 };
            var attempt = new Attempt { UserId = 1, QuizId = 1 };

            _quizRepositoryMock.Setup(r => r.GetQuizById(completeQuizDTO.QuizId)).ReturnsAsync(quiz);
            _userRepositoryMock.Setup(r => r.GetUserById(completeQuizDTO.UserId)).ReturnsAsync(new User());
            _attemptRepositoryMock.Setup(r => r.GetAttemptByUserAndQuiz(completeQuizDTO.UserId, completeQuizDTO.QuizId)).ReturnsAsync(attempt);

            // Act & Assert
            var ex = Assert.ThrowsAsync<UserAttemptException>(() => _quizService.CompleteQuiz(completeQuizDTO));
            Assert.AreEqual("User has already completed this quiz", ex.Message);
        }

        [Test]
        public async Task CompleteQuiz_ReturnsCertificate_WhenAllConditionsMet()
        {
            // Arrange
            var completeQuizDTO = new CompleteQuizDTO { UserId = 1, QuizId = 1, Answers = new List<AnswerDTO>() };
            var quiz = new Quiz { QuizId = 1 };
            var certificate = new Certificate { CertificateId = 1, QuizId = 1, UserId = 1 };
            var certificateDTO = new CertificateDTO { CertificateId = 1, QuizId = 1, UserId = 1 };

            _quizRepositoryMock.Setup(r => r.GetQuizById(completeQuizDTO.QuizId)).ReturnsAsync(quiz);
            _userRepositoryMock.Setup(r => r.GetUserById(completeQuizDTO.UserId)).ReturnsAsync(new User());
            _attemptRepositoryMock.Setup(r => r.GetAttemptByUserAndQuiz(completeQuizDTO.UserId, completeQuizDTO.QuizId)).ReturnsAsync((Attempt)null);
            _certificateRepositoryMock.Setup(r => r.CreateCertificate(It.IsAny<Certificate>())).ReturnsAsync(certificate);
            _mapperMock.Setup(m => m.Map<CertificateDTO>(certificate)).Returns(certificateDTO);

            // Act
            var result = await _quizService.CompleteQuiz(completeQuizDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(certificateDTO.CertificateId, result.CertificateId);
            Assert.AreEqual(certificateDTO.QuizId, result.QuizId);
            Assert.AreEqual(certificateDTO.UserId, result.UserId);
        }
    }
}
