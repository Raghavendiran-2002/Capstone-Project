namespace QuizApi.Exceptions.Quiz
{
    public class QuizNotFoundException : Exception
    {
        public QuizNotFoundException(string? message) : base(message) { }
    }
}
