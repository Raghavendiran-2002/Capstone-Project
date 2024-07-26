using QuizApi.Interfaces.Repository;
using QuizApp.Context;
using QuizApp.Models;

namespace QuizApi.Repositories
{
    public class QuizTagRepository:IQuizTagRepository<string , QuizTag>
    {
        private readonly DBQuizContext _context;
        public QuizTagRepository(DBQuizContext context)
        {
            _context = context;
        }

        public async Task<QuizTag> TagQuiz(QuizTag quizTag)
        {
            _context.QuizTags.Add(quizTag);
            await _context.SaveChangesAsync();
            return quizTag;
        }
    }
}
