namespace QuizApi.Dtos.Profile
{
    public class QuizProfileDTO
    {
        public int QuizId { get; set; }
        public string Topic { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ICollection<QuestionViewProfileDTO> Questions { get; set; }
    }
}
