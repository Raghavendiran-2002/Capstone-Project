using Microsoft.IdentityModel.Tokens;
using QuizApi.Dtos.User;
using QuizApi.Interfaces.Repository;
using QuizApi.Interfaces.Service;
using QuizApp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using QuizApi.Exceptions.User;
namespace QuizApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository<int, User> _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository<int, User> userRepository, IMapper mapper, IConfiguration configuration, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AuthResponseDTO> Register(RegisterUserDTO registerUserDTO)
        {
            var existingUser = await _userRepository.GetUserByEmail(registerUserDTO.Email);
            if (existingUser != null)
            {
                throw new UserAlreadyExistException("User already exists");
            }

            var user = _mapper.Map<User>(registerUserDTO);
            user.CreatedAt = DateTime.UtcNow;

            await _userRepository.AddUser(user);


            var token = GenerateJwtToken(user);
            var userDTO = _mapper.Map<UserDTO>(user);

            return new AuthResponseDTO { Token = token, User = userDTO };
        }

        public async Task<AuthResponseDTO> Login(LoginUserDTO loginUserDTO)
        {
            var user = await _userRepository.GetUserByEmail(loginUserDTO.Email);
            if (user == null || user.Password != loginUserDTO.Password) // Password check should include hashing
            {
                throw new AuthenticationException("Invalid login credentials");
            }

            var token = GenerateJwtToken(user);
            var userDTO = _mapper.Map<UserDTO>(user);

            return new AuthResponseDTO { Token = token, User = userDTO };
        }



        public async Task<bool> ChangePassword(ChangePasswordDTO changePasswordDTO)
        {
            var user = await _userRepository.GetUserByEmail(changePasswordDTO.Email);
            if (user == null || user.Password != changePasswordDTO.OldPassword) // Password check should include hashing
            {
                throw new InvalidPassword("Invalid password");
            }
            if (changePasswordDTO.Name != null)
                user.Name = changePasswordDTO.Name;
            user.Password = changePasswordDTO.NewPassword; // Hash the new password
            await _userRepository.UpdateUser(user);
            return true;
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var userSecret = Environment.GetEnvironmentVariable("JWT_USER_SECRET") ?? "JWT";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(userSecret));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.UserId.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

