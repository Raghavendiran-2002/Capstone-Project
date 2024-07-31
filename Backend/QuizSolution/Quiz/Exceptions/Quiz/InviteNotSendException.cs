namespace QuizApi.Exceptions.Quiz
{
    public class InviteNotSendException : Exception
    {
        public InviteNotSendException(string? message) : base(message) { }
    }
}

