namespace QuizApi.Dtos.User
{
    public class AuthResponseDTO
    {
        public string Token { get; set; }
        public UserDTO User { get; set; }
    }
}
