namespace QuizApi.Exceptions
{
    public class EnvironmentVariableUndefinedException : Exception
    {
        public EnvironmentVariableUndefinedException(string? message) : base(message) { }
    }
}
