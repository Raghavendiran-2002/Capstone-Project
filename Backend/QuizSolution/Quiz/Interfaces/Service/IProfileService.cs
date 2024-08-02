using QuizApi.Dtos.Profile;
using QuizApi.Dtos.Quiz;
using QuizApp.Models;
using System.Collections;

namespace QuizApi.Interfaces.Service
{
    public interface IProfileService
    {
        //Task<ViewProfileDTO> ViewProfile(int userId);
        Task<ViewProfileDTO> ViewProfile(int userId);
        Task<IEnumerable<ViewUpdateQuizDTO>> viewUpdateQuiz(int userId);
        Task<ViewUpdateQuizDTO> UpdateQuiz(ViewUpdateQuizDTO quizDTO);

        Task<QuizDTO> DeleteQuiz(QuizDTO quizDTO);

        Task<bool> UpdateQuizSlot(UpdateQuizSlotDTO quizSlot);

    }
}
