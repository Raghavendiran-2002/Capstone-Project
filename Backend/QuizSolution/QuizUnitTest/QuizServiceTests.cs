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
        public void AttendQuiz_ThrowsUserNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            var attendQuizDTO = new AttendQuizDTO { Email = "nonexistent@example.com", QuizId = 1, Code = "123456" };
            _userRepositoryMock.Setup(r => r.GetUserByEmail(attendQuizDTO.Email)).ReturnsAsync((User)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<UserNotFoundException>(() => _quizService.AttendQuiz(attendQuizDTO));
            Assert.AreEqual("User not Found", ex.Message);
        }

        [Test]
        public void AttendQuiz_ThrowsQuizNotFoundException_WhenQuizDoesNotExist()
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

    }
}
