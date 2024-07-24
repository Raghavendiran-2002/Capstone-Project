using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models
{
    public class Attempt
    {
        [Key]
        public int AttemptId { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }

        public int Score { get; set; }
        public DateTime CompletedAt { get; set; }

        public ICollection<Answer> Answers { get; set; }
        public Certificate Certificate { get; set; }
    }
}
