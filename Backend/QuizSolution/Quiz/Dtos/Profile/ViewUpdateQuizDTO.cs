using QuizApi.Dtos.Quiz;

namespace QuizApi.Dtos.Profile
{
    public class ViewUpdateQuizDTO
    {
        public int QuizId { get; set; }
        public string Topic { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Music { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public List<string>? Tags { get; set; }
        public ICollection<QuestionProfileDTO> Questions { get; set; }
    }
}
