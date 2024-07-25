using System.ComponentModel.DataAnnotations;

namespace QuizApi.Dtos.Quiz
{
    public class CompleteQuizDTO
    {
        [Required(ErrorMessage = "QuizId is required.")]
        public int QuizId { get; set; }

        [Required(ErrorMessage = "EmailId is required.")]
        public string EmailId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public List<UserAnswerDTO> Answers { get; set; }
    }
}
