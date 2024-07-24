using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models
{
    public class Option
    {

        [Key]
        public int OptionId { get; set; }

        [Required]
        public int QuestionId { get; set; }
        public Question Question { get; set; }

        [Required]
        public string OptionText { get; set; }

        public ICollection<CorrectAnswer> CorrectAnswers { get; set; }
        public ICollection<Answer> Answers { get; set; }
    }
}
