using Microsoft.EntityFrameworkCore;
using QuizApi.Interfaces.Repository;
using QuizApp.Context;
using QuizApp.Models;

namespace QuizApi.Repositories
{
    public class QuizRepository : IQuizRepository<int, Quiz>
    {
        private readonly DBQuizContext _context;

        public QuizRepository(DBQuizContext context)
        {
            _context = context;
        }

        public async Task<Quiz> AddQuiz(Quiz quiz)
        {
           _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();
            return quiz;
        }

        public async void DeleteQuizById(int key)
        {
            var quiz = await GetQuizById(key);
            _context.Quizzes.Remove(quiz);
            return;
        }

        public async Task<IEnumerable<Quiz>> GetAllQuiz()
        {
            return await _context.Quizzes.ToListAsync();
        }

        public async Task<Quiz> GetByCode(string code)
        {
           return await _context.Quizzes.FirstAsync(x => x.Code == code);
        }

        public async Task<IEnumerable<Question>> GetQuestionsByQuizId(int quizId)
        {
            return await _context.Questions
           .Where(q => q.QuizId == quizId)
           .Include(q => q.Options)
           .ToListAsync();
        }

        public async Task<Quiz> GetQuizById(int key)
        {
            return await _context.Quizzes.FindAsync(key);
        }

        public async Task<IEnumerable<Quiz>> GetQuizzesByTopicAndTags(string topic, List<string> tags)
        {
            IQueryable<Quiz> query = _context.Quizzes.Include(q => q.QuizTags);

            if (!string.IsNullOrEmpty(topic))
            {
                query = query.Where(q => q.Topic.Contains(topic));
            }

            if (tags != null && tags.Any())
            {
                query = query.Where(q => q.QuizTags.Any(t => tags.Contains(t.Tag)));
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Quiz>> GetQuizzWithQuestions()
        {
            return await _context.Quizzes.Include(q=>q.Questions).ThenInclude(qp=>qp.Options).ToListAsync();
        }

        public async void UpdateQuiz(Quiz quiz)
        {
           if(quiz == null)
                throw new ArgumentNullException(nameof(quiz));
           _context.Quizzes.Update(quiz);
           await _context.SaveChangesAsync();
        }
    }
}
