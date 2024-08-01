using QuizApi.Dtos.User;

namespace QuizApi.Interfaces.Service
{
    public interface IUserService
    {
        Task<AuthResponseDTO> Register(RegisterUserDTO registerUserDTO);
        Task<AuthResponseDTO> Login(LoginUserDTO loginUserDTO);
        Task<bool> ChangePassword(ChangePasswordDTO changePasswordDTO);
    }
}
