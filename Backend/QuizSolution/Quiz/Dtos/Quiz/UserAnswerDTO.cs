using System.ComponentModel.DataAnnotations;

namespace QuizApi.Dtos.Quiz
{
    public class UserAnswerDTO
    {

        [Required(ErrorMessage = "QuestionId is required.")]
        public int QuestionId { get; set; }

        [Required(ErrorMessage = "At least one answer is required.")]
        [MinLength(1, ErrorMessage = "At least one answer is required.")]
        public List<string> SelectedAnswers { get; set; }
    }
}
