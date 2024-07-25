using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizApi.Dtos;
using QuizApi.Dtos.Quiz;
using QuizApi.Dtos.User;
using QuizApi.Exceptions.Quiz;
using QuizApi.Exceptions.User;
using QuizApi.Interfaces.Service;

namespace QuizApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("Cors")]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;
        private readonly ILogger<QuizController> _logger;
        private readonly IMapper _mapper;

        public QuizController(IQuizService quizService, ILogger<QuizController> logger, IMapper mapper)
        {
            _quizService = quizService;
            _logger = logger;
            _mapper = mapper;
        }
        [Authorize]
        [HttpGet("quizzes")]
        [ProducesResponseType(typeof(IEnumerable<QuizDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetQuizzes([FromQuery] string? topic, [FromQuery] List<string> tags)
        {
            try
            {
                var quizzes = await _quizService.GetQuizzes(topic, tags);
                return Ok(quizzes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving quizzes");
                return StatusCode(500, "Internal server error");
            }
        }
        [Authorize]
        [HttpPost("create-quiz")]
        [ProducesResponseType(typeof(QuizDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizDTO createQuizDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var quiz = await _quizService.CreateQuiz(createQuizDTO);
                return Ok(quiz);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating quiz");
                return StatusCode(500, "Internal server error");
            }
        }
        [Authorize]
        [HttpPost("attend-quiz")]
        [ProducesResponseType(typeof(ReturnAttendQuizDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AttendQuiz([FromBody] AttendQuizDTO attendQuizDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _quizService.AttendQuiz(attendQuizDTO);
                return Ok(result);
            }
            catch (UserNotFoundException ex)
            {
                _logger.LogError(ex, ex.Message);
                return NotFound(ex.Message);
            }
            catch (QuizNotFoundException ex)
            {
                _logger.LogError(ex, ex.Message);
                return NotFound(ex.Message);
            }
            catch (InvalidQuizCodeException ex)
            {
                _logger.LogError(ex, ex.Message);
                return ValidationProblem(ex.Message);
            }
            catch (PrivateQuizException ex)
            {
                _logger.LogError(ex, ex.Message);
                return Unauthorized(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("complete-quiz")]
        [ProducesResponseType(typeof(RegisterUserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CompleteQuiz([FromBody] CompleteQuizDTO completeQuizDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _quizService.CompleteQuiz(completeQuizDTO);
                return Ok(result);
            }
            catch (QuizNotFoundException ex)
            {
                _logger.LogError(ex, ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing quiz");
                return StatusCode(500, "Internal server error");
            }
        }     
    }
}
