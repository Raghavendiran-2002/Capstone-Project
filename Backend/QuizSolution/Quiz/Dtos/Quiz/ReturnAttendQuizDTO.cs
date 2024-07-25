namespace QuizApi.Dtos.Quiz
{
    public class ReturnAttendQuizDTO
    {
        public  int QuizId { get; set; }

        public string Emailid { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Duration { get; set; }
        public IEnumerable< QuestionDTO>? QuestionDto {get;set;}
    }
}
