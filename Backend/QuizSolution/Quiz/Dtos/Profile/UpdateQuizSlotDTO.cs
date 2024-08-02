namespace QuizApi.Dtos.Profile
{
    public class UpdateQuizSlotDTO
    {
        public int QuizId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
