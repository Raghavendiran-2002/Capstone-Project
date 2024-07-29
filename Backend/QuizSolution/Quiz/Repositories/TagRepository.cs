using Microsoft.EntityFrameworkCore;
using QuizApi.Interfaces.Repository;
using QuizApp.Context;
using QuizApp.Models;

namespace QuizApi.Repositories
{
    public class TagRepository :ITagRepository<int , Tag>
    {
        private readonly DBQuizContext _context;

        public TagRepository(DBQuizContext context)
        {
            _context = context;
        }

        public async Task<Tag> AddTag(Tag tag)
        {
           _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
            return tag;
        }

        public async  void DeleteTag(Tag tag)
        {
            var tag1 = (await _context.Tags.ToListAsync()).Where(t=>t.TagName == tag.TagName); 
            _context.Tags.Remove(tag);         
        }

        public async Task<IEnumerable<Tag>> GetAllTag()
        {
            var tag = (await _context.Tags.ToListAsync());
            return tag;
        }

        public async Task<Tag> GetTagById(string key)
        {
            return await _context.Tags.FirstOrDefaultAsync(t => t.TagName == key);            
        }
    }
}
