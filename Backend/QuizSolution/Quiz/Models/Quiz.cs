using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models
{
    public class Quiz
    {
        [Key]
        public int QuizId { get; set; }

        [Required]
        public int CreatorId { get; set; }
        public User Creator { get; set; }

        [Required]
        public string Topic { get; set; }

        public string Description { get; set; }
        public int Duration { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Background { get; set; }
        public string? Music { get; set; }
        public string Type { get; set; }
        public string? Code { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<Question> Questions { get; set; }
        public ICollection<QuizTag> QuizTags { get; set; }
        public ICollection<AllowedUser> AllowedUsers { get; set; }
        public ICollection<Attempt> Attempts { get; set; }
        public ICollection<Certificate> Certificates { get; set; }

    }
}
