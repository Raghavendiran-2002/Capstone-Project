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
        [ProducesResponseType(typeof(ReturnAttendQuizDTO), StatusCodes.Status400BadRequest)]
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
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
        [Authorize]
        [HttpPost("update-quiz")]
        [ProducesResponseType(typeof(ViewUpdateQuizDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateQuiz(ViewUpdateQuizDTO quiz)
        {
            try
            {
                var user = await _profileService.UpdateQuiz(quiz);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving quizzes");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
        [Authorize]
        [HttpPost("update-quiz-slot")]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateQuizSlot(UpdateQuizSlotDTO quiz)
        {
            try
            {
                //await _profileService.UpdateQuizSlot(quiz);
                return Ok(new ErrorModel(200,"Updated Successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving quizzes");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
        [Authorize]
        [HttpGet("quiz-creator")]
        [ProducesResponseType(typeof(ViewUpdateQuizDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> QuizCreator(int userId)
        {
            try
            {
                var user = await _profileService.viewUpdateQuiz(userId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving quizzes");
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
    }
}
