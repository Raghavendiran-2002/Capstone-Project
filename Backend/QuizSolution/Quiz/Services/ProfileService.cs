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

        public async Task<ViewUpdateQuizDTO> UpdateQuiz( ViewUpdateQuizDTO quizDTO)
        {
            var quiz = _mapper.Map<Quiz>(quizDTO);                        
            _quizRepository.UpdateQuiz(quiz);
            return quizDTO;
        }
      

        public async Task<ViewProfileDTO> ViewProfile(int userId)
        {
            var user = await _userRepository.GetUserByIdForProfile(userId);
            var userDTO = _mapper.Map<ViewProfileDTO>(user);
            return userDTO;

        }

        public async Task<IEnumerable<ViewUpdateQuizDTO>> viewUpdateQuiz(int userId)
        {
            var quiz = (await  _quizRepository.GetQuizzWithQuestions()).Where(q=>q.CreatorId==userId).ToList();
            var quizDTO = _mapper.Map<IEnumerable<ViewUpdateQuizDTO>>(quiz);
            return quizDTO;
        }
    }
}
