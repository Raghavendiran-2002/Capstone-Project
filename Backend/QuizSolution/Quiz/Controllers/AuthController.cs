using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizApi.Dtos;
using QuizApi.Dtos.User;
using QuizApi.Exceptions.User;
using QuizApi.Interfaces.Service;

namespace QuizApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("Cors")]
    public class AuthController : ControllerBase
    {
    private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;
        private readonly IMapper _mapper;

        public AuthController(IUserService userService, ILogger<AuthController> logger, IMapper mapper)
        {
            _userService = userService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(RegisterUserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO registerUserDTO)
        {
            try
            {
                var result = await _userService.Register(registerUserDTO);
                return Ok(result);
            }
            catch(UserAlreadyExistException e)
            {
                return NotFound(new ErrorModel(505,e.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration");
                return BadRequest(new ErrorModel(401, ex.Message));
            }
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(RegisterUserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status406NotAcceptable)]

        public async Task<IActionResult> Login([FromBody] LoginUserDTO loginUserDTO)
        {
            try
            {
                var result = await _userService.Login(loginUserDTO);
                return Ok(result);
            }
            catch (UserAlreadyExistException e)
            {
                _logger.LogTrace(e, "Auth Failed");
                return Unauthorized(new ErrorModel(406, e.Message));
            }
            catch (AuthenticationException e)
            {
                _logger.LogTrace(e, "Auth Failed");                
                return Unauthorized(new ErrorModel(401, e.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login");
                return BadRequest(new ErrorModel(401,ex.Message));
            }
        }
        [Authorize]
        [HttpPost("change-password")]
        [ProducesResponseType(typeof(ChangePasswordSuccessfullyDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status406NotAcceptable)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("id")?.Value);
                var result = await _userService.ChangePassword(userId, changePasswordDTO);
                return Ok(new ChangePasswordSuccessfullyDTO { status = result });
            }
            catch (InvalidPassword e)
            {
                _logger.LogTrace(e, "Invalid Password");
                return Unauthorized(new ErrorModel(401, e.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during password change");
                return BadRequest(new ErrorModel(401, ex.Message));
            }
        }
    }
}
