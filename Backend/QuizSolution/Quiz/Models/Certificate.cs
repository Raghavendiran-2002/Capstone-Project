using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models
{
    public class Certificate
    {
        [Key]
        public int CertificateId { get; set; }

        [Required]
        public string? CertType { get; set; }

        [Required]
        public int AttemptId { get; set; }
        public Attempt Attempt { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }

        [Required]
        public string Url { get; set; }
    }
}
