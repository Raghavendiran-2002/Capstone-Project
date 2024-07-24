using QuizApp.Models;

namespace QuizApi.Interfaces.Repository
{
    public interface IUserRepository<K, T> where T : class
    {
        Task<T> GetUserByEmail(string email);
        Task<T> GetUserById(K userId);
        Task<T> AddUser(T user);
        Task UpdateUser(T user);
    }
}
