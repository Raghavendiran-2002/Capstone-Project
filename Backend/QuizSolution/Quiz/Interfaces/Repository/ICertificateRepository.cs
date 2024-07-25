namespace QuizApi.Interfaces.Repository
{
    public interface ICertificateRepository<K, T> where T : class
    {
        Task<T> AddCertificate(T certificate);
        Task<IEnumerable<T>> GetCertificateByUserId(int userId);

    }
}
