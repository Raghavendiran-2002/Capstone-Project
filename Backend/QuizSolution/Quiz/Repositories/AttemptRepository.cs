using Microsoft.EntityFrameworkCore;
using QuizApi.Interfaces.Repository;
using QuizApp.Context;
using QuizApp.Models;

namespace QuizApi.Repositories
{
    public class AttemptRepository:IAttemptRepository<int, Attempt>
    {
        private readonly DBQuizContext _context;
        public AttemptRepository(DBQuizContext context) {
            _context = context;
        }

        public async Task<Attempt> AddAttempt(Attempt attempt)
        {
            _context.Attempts.Add(attempt);
            await _context.SaveChangesAsync();
            return attempt;
        }

        public async Task<IEnumerable<Attempt>> GetAttemptByUserId(int userId)
        {
            return await _context.Attempts.Where(a=>a.UserId == userId).ToListAsync();
        }
    }
}
