using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models
{
    public class Answer
    {
        [Key]
        public int AnswerId { get; set; }

        [Required]
        public int AttemptId { get; set; }
        public Attempt Attempt { get; set; }

        [Required]
        public int QuestionId { get; set; }
        public Question Question { get; set; }

        [Required]
        public int OptionId { get; set; }
        public Option Option { get; set; }

    }
}
