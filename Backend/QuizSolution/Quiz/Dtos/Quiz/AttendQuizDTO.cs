using System.ComponentModel.DataAnnotations;

namespace QuizApi.Dtos.Quiz
{
    public class AttendQuizDTO
    {
        [MinLength(6, ErrorMessage = "Quiz Code must be 6 chars long")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Code cannot be empty")]
        public string Code { get; set; }
        [Required]
        public int QuizId { get; set; }
        [Required]
        public string Email { get; set;}

    }
}
