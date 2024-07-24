namespace QuizApi.Exceptions.User
{
    public class InvalidPassword : Exception
    {
        public InvalidPassword(string? message) : base(message) { }
    }
}
