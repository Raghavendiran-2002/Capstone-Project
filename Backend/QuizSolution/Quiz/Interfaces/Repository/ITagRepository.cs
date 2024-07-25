namespace QuizApi.Interfaces.Repository
{
    public interface ITagRepository <K, T> where T : class
    {
        Task<IEnumerable<T>> GetAllTag();
   
        Task<T> AddTag(T tag);
         void  DeleteTag(T tag);
    }
}
