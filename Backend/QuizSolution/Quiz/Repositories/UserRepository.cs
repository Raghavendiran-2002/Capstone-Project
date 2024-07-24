using Microsoft.EntityFrameworkCore;
using QuizApi.Interfaces.Repository;
using QuizApp.Context;
using QuizApp.Models;

namespace QuizApi.Repositories
{
    public class UserRepository : IUserRepository<int, User>
    {
        private readonly DBQuizContext _context;

        public UserRepository(DBQuizContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetUserById(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<User> AddUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task UpdateUser(User user)
        {
            if(user == null)
                throw new ArgumentNullException(nameof(user));
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
