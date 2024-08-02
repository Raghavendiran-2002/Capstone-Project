using QuizApi.Dtos.Quiz;
using QuizApp.Models;

namespace QuizApi.Dtos.Profile
{
    public class ViewProfileDTO
    {
        public string Email { get;set; }
        public string Name { get;set; } = string.Empty;

        public ICollection<QuizProfileDTO> Quizzes { get; set; }
        public ICollection<AttemptProfileDTO> Attempts { get; set; }

    }
}
