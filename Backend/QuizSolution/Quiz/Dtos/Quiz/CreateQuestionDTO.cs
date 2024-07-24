using System.ComponentModel.DataAnnotations;

namespace QuizApi.Dtos.Quiz
{
    public class CreateQuestionDTO
    {
        [Required(ErrorMessage = "Question text is required.")]
        [StringLength(500, ErrorMessage = "Question text cannot exceed 500 characters.")]
        public string QuestionText { get; set; }

        [Required(ErrorMessage = "Options are required.")]
        [MinLength(4, ErrorMessage = "There must be at least four options.")]
        public List<string> Options { get; set; }

        [Required(ErrorMessage = "Correct answers are required.")]
        [MinLength(1, ErrorMessage = "There must be at least one correct answer.")]
        public List<string> CorrectAnswers { get; set; }
    }
}
