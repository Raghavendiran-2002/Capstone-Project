using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models
{
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }

        [Required]
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }

        [Required]
        public string QuestionText { get; set; }
       
        public ICollection<Option> Options { get; set; }
        public ICollection<Answer> Answers { get; set; }
    }
}
