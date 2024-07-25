namespace QuizApi.Dtos.Quiz
{
    public class QuestionDTO
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public List<OptionDTO> Options { get; set; }
    }
}
