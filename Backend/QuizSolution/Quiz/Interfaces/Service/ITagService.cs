using QuizApi.Dtos.Tag;

namespace QuizApi.Interfaces.Service
{
    public interface ITagService
    {
        Task<IEnumerable<TagsDTO>> GetTags();
    }
}
