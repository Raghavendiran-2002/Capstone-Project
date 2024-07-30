namespace QuizApi.Dtos.Quiz
{
    public class QuizDTO
    {
        public int QuizId { get; set; }
        public string Topic { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }

        public  bool DurationPerQuestion { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }  
        public string Type { get; set; }
        public string Code { get; set; }
       // public List<string> Tags { get; set; }
    }
}
