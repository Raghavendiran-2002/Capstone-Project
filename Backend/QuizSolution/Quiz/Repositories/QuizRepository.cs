using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuizApi.Dtos.Profile;
using QuizApi.Exceptions.Quiz;
using QuizApi.Interfaces.Repository;
using QuizApp.Context;
using QuizApp.Models;

namespace QuizApi.Repositories
{
    public class QuizRepository : IQuizRepository<int, Quiz>
    {
        private readonly DBQuizContext _context;
        private readonly IMapper _mapper;
        public QuizRepository(DBQuizContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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

        public async void UpdateQuiz(ViewUpdateQuizDTO viewUpdateQuizDTO)
        {
            var quiz = await _context.Quizzes             
             .FirstOrDefaultAsync(q => q.QuizId == viewUpdateQuizDTO.QuizId);

            if (quiz == null)
            {
                throw new QuizNotFoundException("Quiz Not Found");
            }

            
            _mapper.Map(viewUpdateQuizDTO, quiz);

       
            foreach (var question in quiz.Questions)
            {
                _context.Options.RemoveRange(question.Options);
            }
            _context.Questions.RemoveRange(quiz.Questions);


            quiz.Questions = viewUpdateQuizDTO.Questions
                .Select(q => _mapper.Map<Question>(q))
                .ToList();
            _context.Update(quiz);
            await _context.SaveChangesAsync();
        }

        public async void UpdateQuizSlotRepo(UpdateQuizSlotDTO quiz)
        {
            var updatequiz = await GetQuizById(quiz.QuizId);

            if (updatequiz == null)
            {
                throw new QuizNotFoundException("Quiz not found");
            }
            updatequiz.StartTime = quiz.StartTime;
            updatequiz.EndTime = quiz.EndTime;
            _context.Quizzes.Update(updatequiz);
            await _context.SaveChangesAsync();
        }
    }
}
