namespace QuizApi.Interfaces.Repository
{
    public interface IAllowedUserRepository<K, T> where T : class
    {
        Task<IEnumerable<T>> GetAllowedUserById(int userId);
        Task<IEnumerable<T>> GetAllowedUserByQuiz(int quizId);
        Task <IEnumerable<T>> ValidateQuizAccess(int userId, int quizId);
    }
}
