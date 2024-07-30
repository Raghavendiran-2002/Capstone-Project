namespace QuizApi.Dtos.Quiz
{
    public class GenerateCertReturnDTO
    {
        public string Name { get; set; }
        public string IssueDate { get; set; }
        public string ExpDate { get; set; }
        public Double score { get; set; }
        public string certType { get; set; }

        public string pdfUrl { get; set; }  
    }
}
