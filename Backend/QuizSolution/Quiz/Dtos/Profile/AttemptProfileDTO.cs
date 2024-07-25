using QuizApp.Models;

namespace QuizApi.Dtos.Profile
{
    public class AttemptProfileDTO
    {
        public int AttemptId { get; set; }

        public int QuizId { get; set; }
        public int Score { get; set; }
        public DateTime CompletedAt { get; set; }
        public CertificateProfileDTO Certificate { get; set; }

    }
}
