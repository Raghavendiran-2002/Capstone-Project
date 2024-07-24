using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models
{
    public class CorrectAnswer
    {
        [Key]
        public int CorrectAnswerId { get; set; }

        [Required]
        public int QuestionId { get; set; }
        public Question Question { get; set; }

        [Required]
        public int OptionId { get; set; }
        public Option Option { get; set; }
    }
}
