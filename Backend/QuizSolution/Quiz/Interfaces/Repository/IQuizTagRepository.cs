namespace QuizApi.Interfaces.Repository
{
    public interface IQuizTagRepository<K, T> where T : class
    {
        Task<T> TagQuiz(T quizTag);
    }
}
