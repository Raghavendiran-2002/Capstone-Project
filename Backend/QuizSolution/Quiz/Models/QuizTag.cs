using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models
{
    public class QuizTag
    {
        [Required]
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }

        [Required]
        public string Tag { get; set; }
        public Tag TagEntity { get; set; }

    }
}
