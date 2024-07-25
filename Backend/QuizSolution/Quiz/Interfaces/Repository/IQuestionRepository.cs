namespace QuizApi.Interfaces.Repository
{
    public interface IQuestionRepository<K, T> where T : class
    {
        Task<T> GetQuestionByQuizId(int quizId);
        Task<K> GetQuestionCount(int quizId);
    }
}
