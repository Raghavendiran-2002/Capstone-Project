namespace QuizApi.Dtos.Quiz
{
    public class ReturnCompleteQuizDTO
    {
        public string? QuizTopic {get;set;}
        public double Score { get; set; }
        public string Status { get; set; }
        public string CertUrl { get; set;}  
    }
}
