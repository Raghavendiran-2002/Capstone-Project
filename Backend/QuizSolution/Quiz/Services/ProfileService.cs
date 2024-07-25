using AutoMapper;
using QuizApi.Dtos.Profile;
using QuizApi.Dtos.Quiz;
using QuizApi.Dtos.User;
using QuizApi.Exceptions.Profile;
using QuizApi.Interfaces.Repository;
using QuizApi.Interfaces.Service;
using QuizApp.Models;
namespace QuizApi.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserRepository<int, User> _userRepository;
        private readonly IMapper _mapper;
        private readonly IQuizRepository<int, Quiz> _quizRepository;

        public ProfileService(IQuizRepository<int, Quiz> quizRepository,IUserRepository<int, User> userRepository, IMapper mapper) {
            _userRepository = userRepository;
            _mapper = mapper;
            _quizRepository = quizRepository;
        }
        
        public async Task<QuizDTO> DeleteQuiz(QuizDTO quizDTO)
        {
            _quizRepository.DeleteQuizById(quizDTO.QuizId);
            return quizDTO;
        }

        public async Task<QuizDTO> UpdateQuiz(int userId,QuizDTO quizDTO)
        {            
            var quiz = await _quizRepository.GetQuizById(quizDTO.QuizId);
            if (quiz.CreatorId != userId)
                throw new NotTheCreatorOfQuiz("No Creator of Quiz");
            quiz = _mapper.Map<Quiz>(quizDTO);
            _quizRepository.UpdateQuiz(quiz);
            return quizDTO;
        }
      

        public async Task<ViewProfileDTO> ViewProfile(int userId)
        {
            var user = await _userRepository.GetUserByIdForProfile(userId);
            var userDTO = _mapper.Map<ViewProfileDTO>(user);

            return userDTO;

        }
    }
}
