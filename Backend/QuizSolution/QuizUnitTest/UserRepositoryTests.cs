using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using QuizApp.Context;
using QuizApp.Models;
using QuizApi.Repositories;

namespace QuizApi.Tests.Repositories
{
    [TestFixture]
    public class UserRepositoryTests
    {
        private Mock<DBQuizContext> _mockContext;
        private UserRepository _repository;
        private Mock<DbSet<User>> _mockSet;

        [SetUp]
        public void Setup()
        {
            _mockContext = new Mock<DBQuizContext>();
            _mockSet = new Mock<DbSet<User>>();
            _mockContext.Setup(m => m.Users).Returns(_mockSet.Object);

            _repository = new UserRepository(_mockContext.Object);
        }

        [Test]
        public async Task GetUserByEmail_UserExists_ReturnsUser()
        {
            // Arrange
            var email = "test@example.com";
            var user = new User { Email = email };
            var data = new List<User> { user }.AsQueryable();

            _mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(data.Provider);
            _mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(data.Expression);
            _mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            // Act
            var result = await _repository.GetUserByEmail(email);

            // Assert
            Assert.AreEqual(user, result);
        }

        [Test]
        public async Task GetUserById_UserExists_ReturnsUser()
        {
            // Arrange
            var userId = 1;
            var user = new User { UserId = userId };

            _mockSet.Setup(m => m.FindAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _repository.GetUserById(userId);

            // Assert
            Assert.AreEqual(user, result);
        }

        [Test]
        public async Task AddUser_ValidUser_AddsUser()
        {
            // Arrange
            var user = new User { UserId = 1 };

            // Act
            var result = await _repository.AddUser(user);

            // Assert
            _mockSet.Verify(m => m.Add(It.IsAny<User>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.AreEqual(user, result);
        }

        [Test]
        public void UpdateUser_NullUser_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _repository.UpdateUser(null));
        }

        [Test]
        public async Task UpdateUser_ValidUser_UpdatesUser()
        {
            // Arrange
            var user = new User { UserId = 1 };

            // Act
            await _repository.UpdateUser(user);

            // Assert
            _mockSet.Verify(m => m.Update(It.IsAny<User>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetUserByIdForProfile_UserExists_ReturnsUser()
        {
            // Arrange
            var userId = 1;
            var user = new User { UserId = userId };
            var data = new List<User> { user }.AsQueryable();

            _mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(data.Provider);
            _mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(data.Expression);
            _mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            _mockSet.Setup(m => m.Include(It.IsAny<string>())).Returns(_mockSet.Object);

            // Act
            var result = await _repository.GetUserByIdForProfile(userId);

            // Assert
            Assert.AreEqual(user, result);
        }
    }
}
