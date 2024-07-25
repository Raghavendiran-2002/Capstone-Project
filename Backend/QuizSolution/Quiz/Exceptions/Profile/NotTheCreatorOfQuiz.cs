namespace QuizApi.Exceptions.Profile
{
    public class NotTheCreatorOfQuiz
   : Exception
    {
        public NotTheCreatorOfQuiz(string? message) : base(message) { }
    }
}
