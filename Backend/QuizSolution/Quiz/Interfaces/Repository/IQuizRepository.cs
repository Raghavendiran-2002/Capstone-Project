using QuizApp.Models;

namespace QuizApi.Interfaces.Repository
{
    public interface IQuizRepository<K, T> where T : class
    {
        Task<T> GetQuizById(K key);
        Task<IEnumerable<T>> GetAllQuiz();
        Task<T> AddQuiz(T quiz);
        void UpdateQuiz(T quiz);
        void DeleteQuizById(K key);
        Task<IEnumerable<T>> GetQuizzesByTopicAndTags(string topic, List<string> tags);

        Task<T> GetByCode(string code);

     }
}
