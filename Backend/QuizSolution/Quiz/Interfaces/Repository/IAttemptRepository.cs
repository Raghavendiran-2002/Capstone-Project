namespace QuizApi.Interfaces.Repository
{
    public interface IAttemptRepository<K, T> where T : class
    {
        Task<T> AddAttempt(T attempt);
        Task<IEnumerable<T>> GetAttemptByUserId(int userId);
        
    }
}
