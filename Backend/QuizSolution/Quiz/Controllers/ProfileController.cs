using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizApi.Dtos.Quiz;
using QuizApi.Dtos;
using QuizApi.Interfaces.Service;
using QuizApi.Services;
using Microsoft.AspNetCore.Cors;
using QuizApi.Dtos.Profile;

namespace QuizApi.Controllers
{
    [EnableCors("Cors")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;
        private readonly ILogger<QuizController> _logger;
        private readonly IMapper _mapper;
        public ProfileController(IProfileService profileService, ILogger<QuizController> logger, IMapper mapper)
        {
            _profileService = profileService;
            _logger = logger;
            _mapper = mapper;
        }
        [Authorize]
        [HttpGet("view-profile")]
        [ProducesResponseType(typeof(ViewProfileDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserProfile(int userId)
        {
            try
            {
                var user = await _profileService.ViewProfile(userId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving quizzes");
                return StatusCode(500, ex.Message);
            }
        }
        [Authorize]
        [HttpPost("update-quiz")]
        [ProducesResponseType(typeof(QuizDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateQuiz(int userId,QuizDTO quiz)
        {
            try
            {
                var user = await _profileService.UpdateQuiz(userId,quiz);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving quizzes");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
