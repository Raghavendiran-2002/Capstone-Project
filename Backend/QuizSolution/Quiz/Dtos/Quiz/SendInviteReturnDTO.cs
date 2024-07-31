using System.ComponentModel.DataAnnotations;

namespace QuizApi.Dtos.Quiz
{
    public class SendInviteReturnDTO
    {
        
        public int status { get; set; }

        
        public bool isSend { get; set; }

    }
}
