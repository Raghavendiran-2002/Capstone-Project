using System.ComponentModel.DataAnnotations;

namespace QuizApi.Dtos.Quiz
{
    public class SendInviteDTO
    {

     
        public int quizId { get; set; }

        public string subject { get; set; }
        
        public string body { get; set; }
  
        public IEnumerable<String> recipients { get; set; }

    }
}
