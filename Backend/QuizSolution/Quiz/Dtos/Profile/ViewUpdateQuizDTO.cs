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
        public ICollection<QuestionProfileDTO> Questions { get; set; }
    }
}
