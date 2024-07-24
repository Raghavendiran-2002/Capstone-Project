namespace QuizApi.Exceptions.User
{
    public class AuthenticationException : Exception
    {
        public AuthenticationException(string? message) : base(message) { }
    }
}
