using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using QuizApi.Dtos.Profile;
using QuizApi.Dtos.Quiz;
using QuizApi.Dtos.User;
using QuizApi.Interfaces.Repository;
using QuizApi.Interfaces.Service;
using QuizApi.Services;
using QuizApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

[TestFixture]
public class ProfileServiceTests
{
    private Mock<IUserRepository<int, User>> _userRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private Mock<IQuizRepository<int, Quiz>> _quizRepositoryMock;
    private Mock<IDistributedCache> _cacheMock;
    private Mock<ILogger<UserService>> _loggerMock;
    private ProfileService _profileService;

    [SetUp]
    public void SetUp()
    {
        _userRepositoryMock = new Mock<IUserRepository<int, User>>();
        _mapperMock = new Mock<IMapper>();
        _quizRepositoryMock = new Mock<IQuizRepository<int, Quiz>>();
        _cacheMock = new Mock<IDistributedCache>();
        _loggerMock = new Mock<ILogger<UserService>>();

        _profileService = new ProfileService(
            _quizRepositoryMock.Object,
            _loggerMock.Object,
            _cacheMock.Object,
            _userRepositoryMock.Object,
            _mapperMock.Object
        );
    }

    [Test]
    public async Task DeleteQuiz_ShouldDeleteQuizAndReturnQuizDTO()
    {
        // Arrange
        var quizDTO = new QuizDTO { QuizId = 1 };

        // Act
        var result = await _profileService.DeleteQuiz(quizDTO);

        // Assert
        _quizRepositoryMock.Verify(q => q.DeleteQuizById(quizDTO.QuizId), Times.Once);
        Assert.AreEqual(quizDTO, result);
    }

    [Test]
    public async Task UpdateQuiz_ShouldUpdateQuizAndReturnQuizDTO()
    {
        // Arrange
        var quizDTO = new ViewUpdateQuizDTO { QuizId = 1 };
        var quiz = new Quiz { QuizId = 1 };
        _mapperMock.Setup(m => m.Map<Quiz>(quizDTO)).Returns(quiz);

        // Act
        var result = await _profileService.UpdateQuiz(quizDTO);

        // Assert
        _quizRepositoryMock.Verify(q => q.UpdateQuiz(quizDTO), Times.Once);
        Assert.AreEqual(quizDTO, result);
    }

    [Test]
    public async Task UpdateQuizSlot_ShouldUpdateQuizSlotAndReturnTrue()
    {
        // Arrange
        var quizSlot = new UpdateQuizSlotDTO { QuizId = 1, Slot = 2 };

        // Act
        var result = await _profileService.UpdateQuizSlot(quizSlot);

        // Assert
        _quizRepositoryMock.Verify(q => q.UpdateQuizSlotRepo(quizSlot), Times.Once);
        Assert.IsTrue(result);
    }

    [Test]
    public async Task ViewProfile_ShouldReturnProfileFromCacheIfExists()
    {
        // Arrange
        var userId = 1;
        var cachedProfile = new ViewProfileDTO { UserId = userId };
        _cacheMock.Setup(c => c.GetStringAsync(It.IsAny<string>()))
                  .ReturnsAsync(JsonSerializer.Serialize(cachedProfile));

        // Act
        var result = await _profileService.ViewProfile(userId);

        // Assert
        _cacheMock.Verify(c => c.GetStringAsync(It.IsAny<string>()), Times.Once);
        _loggerMock.Verify(l => l.LogInformation($"Cache hit for user profile {userId}"), Times.Once);
        Assert.AreEqual(cachedProfile.UserId, result.UserId);
    }

    [Test]
    public async Task ViewProfile_ShouldReturnProfileFromRepositoryIfNotInCache()
    {
        // Arrange
        var userId = 1;
        var user = new User { UserId = userId };
        var userDTO = new ViewProfileDTO { UserId = userId };

        _cacheMock.Setup(c => c.GetStringAsync(It.IsAny<string>())).ReturnsAsync((string)null);
        _userRepositoryMock.Setup(r => r.GetUserByIdForProfile(userId)).ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<ViewProfileDTO>(user)).Returns(userDTO);

        // Act
        var result = await _profileService.ViewProfile(userId);

        // Assert
        _cacheMock.Verify(c => c.GetStringAsync(It.IsAny<string>()), Times.Once);
        _loggerMock.Verify(l => l.LogInformation($"Cache miss for user profile {userId}"), Times.Once);
        _userRepositoryMock.Verify(r => r.GetUserByIdForProfile(userId), Times.Once);
        _cacheMock.Verify(c => c.SetStringAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
        Assert.AreEqual(userDTO.UserId, result.UserId);
    }

    [Test]
    public async Task ViewProfile_ShouldReturnNullIfUserNotFound()
    {
        // Arrange
        var userId = 1;

        _cacheMock.Setup(c => c.GetStringAsync(It.IsAny<string>())).ReturnsAsync((string)null);
        _userRepositoryMock.Setup(r => r.GetUserByIdForProfile(userId)).ReturnsAsync((User)null);

        // Act
        var result = await _profileService.ViewProfile(userId);

        // Assert
        _cacheMock.Verify(c => c.GetStringAsync(It.IsAny<string>()), Times.Once);
        _loggerMock.Verify(l => l.LogInformation($"Cache miss for user profile {userId}"), Times.Once);
        _userRepositoryMock.Verify(r => r.GetUserByIdForProfile(userId), Times.Once);
        _loggerMock.Verify(l => l.LogWarning($"User {userId} not found"), Times.Once);
        Assert.IsNull(result);
    }

    [Test]
    public async Task ViewUpdateQuiz_ShouldReturnQuizzesForUser()
    {
        // Arrange
        var userId = 1;
        var quizzes = new List<Quiz>
        {
            new Quiz { QuizId = 1, CreatorId = userId },
            new Quiz { QuizId = 2, CreatorId = userId }
        };
        var quizDTOs = quizzes.Select(q => new ViewUpdateQuizDTO { QuizId = q.QuizId }).ToList();

        _quizRepositoryMock.Setup(r => r.GetQuizzWithQuestions()).ReturnsAsync(quizzes);
        _mapperMock.Setup(m => m.Map<IEnumerable<ViewUpdateQuizDTO>>(It.IsAny<IEnumerable<Quiz>>())).Returns(quizDTOs);

        // Act
        var result = await _profileService.viewUpdateQuiz(userId);

        // Assert
        _quizRepositoryMock.Verify(r => r.GetQuizzWithQuestions(), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<ViewUpdateQuizDTO>>(It.IsAny<IEnumerable<Quiz>>()), Times.Once);
        Assert.AreEqual(quizDTOs.Count, result.Count());
    }
}
