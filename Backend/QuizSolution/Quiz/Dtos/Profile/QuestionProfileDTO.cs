using QuizApi.Dtos.Quiz;

namespace QuizApi.Dtos.Profile
{
    public class QuestionProfileDTO
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public List<OptionWithAnswerDTO> Options { get; set; }
    }
}
