using Microsoft.EntityFrameworkCore;
using QuizApi.Interfaces.Repository;
using QuizApp.Context;
using QuizApp.Models;

namespace QuizApi.Repositories
{
    public class AllowedUserRepository: IAllowedUserRepository<int, AllowedUser> 
    {
        private readonly DBQuizContext _context;
        public AllowedUserRepository(DBQuizContext context) {
            _context = context;
        }

        public async Task<IEnumerable<AllowedUser>> GetAllowedUserById(int userId)
        {
            return await _context.AllowedUsers.Where(a=>a.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<AllowedUser>> GetAllowedUserByQuiz(int quizId)
        {
            return await _context.AllowedUsers.Where(a => a.QuizId == quizId).ToListAsync();
        }

        public async Task<IEnumerable<AllowedUser>> ValidateQuizAccess(int userId, int quizId)
        {
            return (await _context.AllowedUsers.ToListAsync()).Where(q => (q.QuizId == quizId)).Where(p=>(p.UserId == userId));
        }
    }
}
