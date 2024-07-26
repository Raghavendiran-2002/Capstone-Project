namespace QuizApi.Dtos.Profile
{
    public class OptionWithAnswerDTO
    {
        public int OptionId { get; set; }
        public string OptionText { get; set; }
        public bool IsAnswer { get; set; }
    }
}
