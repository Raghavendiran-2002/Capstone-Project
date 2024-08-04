using AutoMapper;
using QuizApi.Dtos.Profile;
using QuizApi.Dtos.Quiz;
using QuizApi.Dtos.User;
using QuizApi.Exceptions.Profile;
using QuizApi.Interfaces.Repository;
using QuizApi.Interfaces.Service;
using QuizApp.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
namespace QuizApi.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserRepository<int, User> _userRepository;
        private readonly IMapper _mapper;
        private readonly IQuizRepository<int, Quiz> _quizRepository;
        private readonly IDistributedCache _cache;
        private readonly ILogger<UserService> _logger;
        public ProfileService(IQuizRepository<int, Quiz> quizRepository, ILogger<UserService> logger, IDistributedCache cache, IUserRepository<int, User> userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _logger = logger;
            _mapper = mapper;
            _cache = cache;
            _quizRepository = quizRepository;
        }

        public async Task<QuizDTO> DeleteQuiz(QuizDTO quizDTO)
        {
            _quizRepository.DeleteQuizById(quizDTO.QuizId);
            return quizDTO;
        }

        public async Task<ViewUpdateQuizDTO> UpdateQuiz(ViewUpdateQuizDTO quizDTO)
        {
            var quiz = _mapper.Map<Quiz>(quizDTO);
            _quizRepository.UpdateQuiz(quizDTO);
            return quizDTO;
        }

        public async Task<bool> UpdateQuizSlot(UpdateQuizSlotDTO quizSlot)
        {
            _quizRepository.UpdateQuizSlotRepo(quizSlot);
            return true;
        }

        public async Task<ViewProfileDTO> ViewProfile(int userId)
        {
            var cacheKey = $"user_profile_{userId}";
            var cachedProfile = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedProfile))
            {
                _logger.LogInformation($"Cache hit for user profile {userId}");
                return JsonSerializer.Deserialize<ViewProfileDTO>(cachedProfile);
            }

            _logger.LogInformation($"Cache miss for user profile {userId}");
            var user = await _userRepository.GetUserByIdForProfile(userId);
            if (user == null)
            {
                _logger.LogWarning($"User {userId} not found");
                return null; // or throw an appropriate exception
            }

            var userDTO = _mapper.Map<ViewProfileDTO>(user);

            var cacheOptions = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10))
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(userDTO), cacheOptions);

            return userDTO;

        }

        public async Task<IEnumerable<ViewUpdateQuizDTO>> viewUpdateQuiz(int userId)
        {
            var quiz = (await _quizRepository.GetQuizzWithQuestions()).Where(q => q.CreatorId == userId).ToList();
            var quizDTO = _mapper.Map<IEnumerable<ViewUpdateQuizDTO>>(quiz);
            return quizDTO;
        }
    }
}
