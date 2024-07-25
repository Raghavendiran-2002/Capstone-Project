namespace QuizApi.Exceptions.Quiz
{
    public class PrivateQuizException: Exception
    {
        public PrivateQuizException(string? message) : base(message) { }
    }
}
