using System.ComponentModel.DataAnnotations;

namespace QuizApi.Dtos.Quiz
{
    public class SendInviteDTO
    {


        public int quizId { get; set; }

        public string quizCode { get; set; }

        public string usertype { get; set; }

        public IEnumerable<String> recipients { get; set; }

    }
}
