using System.ComponentModel.DataAnnotations;

namespace QuizApi.Dtos.Profile
{
    public class CertificateProfileDTO
    {
        [Required]
        public int CertificateId { get; set; }

        [Required]
        public string? CertType { get; set; }
        [Required]
        public string Url { get; set; }
    }
}
