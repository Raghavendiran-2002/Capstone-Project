namespace QuizApi.Exceptions.Quiz
{
    public class TimeLimitExceededException : Exception
    {
        public TimeLimitExceededException(string? message) : base(message) { }
    }
}
