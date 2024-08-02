namespace QuizApi.Dtos.Profile
{
    public class QuestionViewProfileDTO
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public ICollection<OptionProfileDTO> Options { get; set; }
    }
}
