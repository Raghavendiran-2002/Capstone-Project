namespace QuizApi.Exceptions.Quiz
{
    public class QuizTimeException: Exception
    {
        public QuizTimeException(string? message) : base(message) { }
    }
}
