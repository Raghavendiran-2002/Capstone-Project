namespace QuizApi.Exceptions.User
{
    public class UserAlreadyExistException : Exception
    {
        public UserAlreadyExistException(string? message) : base(message) { }
    }
}
