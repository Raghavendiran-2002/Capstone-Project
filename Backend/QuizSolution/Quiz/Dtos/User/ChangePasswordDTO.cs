using System.ComponentModel.DataAnnotations;

namespace QuizApi.Dtos.User
{
    public class ChangePasswordDTO
    {
        [MinLength(6, ErrorMessage = "Password has to be minmum 6 chars long")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password cannot be empty")]
        public string OldPassword { get; set; }
        [MinLength(6, ErrorMessage = "Password has to be minmum 6 chars long")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password cannot be empty")]
        public string NewPassword { get; set; }
    }
}
