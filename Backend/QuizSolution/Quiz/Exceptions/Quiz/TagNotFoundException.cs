namespace QuizApi.Exceptions.Quiz
{
    public class TagNotFoundException
  : Exception
    {
        public TagNotFoundException(string? message) : base(message) { }
    }
}
