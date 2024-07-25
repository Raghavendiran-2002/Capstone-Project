using Microsoft.EntityFrameworkCore;
using QuizApi.Interfaces.Repository;
using QuizApp.Context;
using QuizApp.Models;

namespace QuizApi.Repositories
{
    public class CertificateRepository : ICertificateRepository<int, Certificate>
    {
        private readonly DBQuizContext _context;

        public CertificateRepository(DBQuizContext context)
        {
            _context = context;
        }

        public async Task<Certificate> AddCertificate(Certificate certificate)
        {
            _context.Certificates.Add(certificate);
            await _context.SaveChangesAsync();
            return certificate;
        }

        public  async Task<IEnumerable<Certificate>> GetCertificateByUserId(int userId)
        {
            return await _context.Certificates.Where(a => a.UserId == userId).ToListAsync();
        }
    }
}
