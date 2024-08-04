using NUnit.Framework;
using Moq;
using QuizApi.Interfaces.Repository;
using QuizApi.Interfaces.Service;
using QuizApi.Services;
using QuizApi.Dtos.User;
using QuizApp.Models;
using QuizApi.Exceptions.User;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace QuizApi.Tests.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUserRepository<int, User>> _mockUserRepository;
        private Mock<IMapper> _mockMapper;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<ILogger<UserService>> _mockLogger;
        private IUserService _userService;

        [SetUp]
        public void Setup()
        {
            _mockUserRepository = new Mock<IUserRepository<int, User>>();
            _mockMapper = new Mock<IMapper>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<UserService>>();
            _userService = new UserService(_mockUserRepository.Object, _mockMapper.Object, _mockConfiguration.Object, _mockLogger.Object);
        }

        #region Register Tests

        [Test]
        public async Task Register_UserAlreadyExists_ThrowsUserAlreadyExistException()
        {
            var registerUserDTO = new RegisterUserDTO { Email = "test@example.com" };
            _mockUserRepository.Setup(x => x.GetUserByEmail(registerUserDTO.Email)).ReturnsAsync(new User());

            Assert.ThrowsAsync<UserAlreadyExistException>(async () => await _userService.Register(registerUserDTO));
        }

        [Test]
        public async Task Register_ValidUser_ReturnsAuthResponseDTO()
        {
            var registerUserDTO = new RegisterUserDTO { Email = "test@example.com" };
            var user = new User();
            var userDTO = new UserDTO();
            _mockUserRepository.Setup(x => x.GetUserByEmail(registerUserDTO.Email)).ReturnsAsync((User)null);
            _mockMapper.Setup(x => x.Map<User>(registerUserDTO)).Returns(user);
            _mockUserRepository.Setup(x => x.AddUser(user)).Returns(Task.CompletedTask);
            _mockMapper.Setup(x => x.Map<UserDTO>(user)).Returns(userDTO);

            var result = await _userService.Register(registerUserDTO);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Token);
            Assert.AreEqual(result.User, userDTO);
        }

        #endregion

        #region Login Tests

        [Test]
        public async Task Login_InvalidCredentials_ThrowsAuthenticationException()
        {
            var loginUserDTO = new LoginUserDTO { Email = "test@example.com", Password = "wrongpassword" };
            _mockUserRepository.Setup(x => x.GetUserByEmail(loginUserDTO.Email)).ReturnsAsync((User)null);

            Assert.ThrowsAsync<AuthenticationException>(async () => await _userService.Login(loginUserDTO));
        }

        [Test]
        public async Task Login_ValidCredentials_ReturnsAuthResponseDTO()
        {
            var loginUserDTO = new LoginUserDTO { Email = "test@example.com", Password = "password" };
            var user = new User { Password = "password" }; // Password should be hashed in real scenario
            var userDTO = new UserDTO();
            _mockUserRepository.Setup(x => x.GetUserByEmail(loginUserDTO.Email)).ReturnsAsync(user);
            _mockMapper.Setup(x => x.Map<UserDTO>(user)).Returns(userDTO);

            var result = await _userService.Login(loginUserDTO);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Token);
            Assert.AreEqual(result.User, userDTO);
        }

        #endregion

        #region ChangePassword Tests

        [Test]
        public async Task ChangePassword_InvalidOldPassword_ThrowsInvalidPassword()
        {
            var changePasswordDTO = new ChangePasswordDTO { Email = "test@example.com", OldPassword = "wrongpassword", NewPassword = "newpassword" };
            var user = new User { Password = "password" };
            _mockUserRepository.Setup(x => x.GetUserByEmail(changePasswordDTO.Email)).ReturnsAsync(user);

            Assert.ThrowsAsync<InvalidPassword>(async () => await _userService.ChangePassword(changePasswordDTO));
        }

        [Test]
        public async Task ChangePassword_ValidPassword_ReturnsTrue()
        {
            var changePasswordDTO = new ChangePasswordDTO { Email = "test@example.com", OldPassword = "password", NewPassword = "newpassword", Name = "New Name" };
            var user = new User { Password = "password" }; // Password should be hashed in real scenario
            _mockUserRepository.Setup(x => x.GetUserByEmail(changePasswordDTO.Email)).ReturnsAsync(user);

            var result = await _userService.ChangePassword(changePasswordDTO);

            Assert.IsTrue(result);
        }

        #endregion
    }
}
