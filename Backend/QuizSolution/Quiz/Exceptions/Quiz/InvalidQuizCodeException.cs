namespace QuizApi.Exceptions.Quiz
{
    public class InvalidQuizCodeException
 : Exception
    {
        public InvalidQuizCodeException(string? message) : base(message) { }
    }
}
