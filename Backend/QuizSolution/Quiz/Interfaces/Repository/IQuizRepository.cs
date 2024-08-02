using QuizApi.Dtos.Profile;
using QuizApp.Models;

namespace QuizApi.Interfaces.Repository
{
    public interface IQuizRepository<K, T> where T : class
    {
        Task<T> GetQuizById(K key);
        Task<IEnumerable<T>> GetAllQuiz();
        Task<IEnumerable<T>> GetQuizzWithQuestions();
        Task<T> AddQuiz(T quiz);
        void UpdateQuiz(ViewUpdateQuizDTO quiz);
        void DeleteQuizById(K key);
        Task<IEnumerable<T>> GetQuizzesByTopicAndTags(string topic, List<string> tags);
        void UpdateQuizSlotRepo(UpdateQuizSlotDTO quiz);
        Task<T> GetByCode(string code);
        Task<IEnumerable<Question>> GetQuestionsByQuizId(int quizId);


    }
}
