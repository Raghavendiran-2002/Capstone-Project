using QuizApi.Dtos.Quiz;

namespace QuizApi.Interfaces.Service
{
    public interface IQuizService
    {
        Task<IEnumerable<QuizDTO>> GetQuizzes(string topic = null, List<string> tags = null);
        Task<QuizDTO> CreateQuiz(CreateQuizDTO createQuizDTO);
        Task<ReturnAttendQuizDTO> AttendQuiz(AttendQuizDTO attendQuizDTO);
        Task<bool> CompleteQuiz(CompleteQuizDTO completeQuizDTO);
        Task<IEnumerable<QuestionDTO>> GetQuizQuestions(int quizId);

    }
}
