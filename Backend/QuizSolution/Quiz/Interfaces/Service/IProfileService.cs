using QuizApi.Dtos.Profile;
using QuizApi.Dtos.Quiz;
using QuizApp.Models;

namespace QuizApi.Interfaces.Service
{
    public interface IProfileService
    {
        //Task<ViewProfileDTO> ViewProfile(int userId);
        Task<ViewProfileDTO> ViewProfile(int userId);

        Task<QuizDTO> UpdateQuiz(int userId,QuizDTO quizDTO);

        Task<QuizDTO> DeleteQuiz(QuizDTO quizDTO);

    }
}
