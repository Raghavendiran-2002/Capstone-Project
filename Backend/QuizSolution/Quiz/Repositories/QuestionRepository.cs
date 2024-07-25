using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using QuizApi.Interfaces.Repository;
using QuizApp.Context;
using QuizApp.Models;

namespace QuizApi.Repositories
{
    public class QuestionRepository: IQuestionRepository<int, Question>
    {
        private readonly DBQuizContext _context;

        public QuestionRepository(DBQuizContext context)
        {
            _context = context;
        }

        public async Task<Question> GetQuestionByQuizId(int quizId)
        {
            return await _context.Questions.Include(q=>q.Options).FirstOrDefaultAsync(a=>a.QuizId == quizId);
        }

        public async Task<int> GetQuestionCount(int quizId)
        {
            return await _context.Questions.CountAsync();
        }
    }
}
