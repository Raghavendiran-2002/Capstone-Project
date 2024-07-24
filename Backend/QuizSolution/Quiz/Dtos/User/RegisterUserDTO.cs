using System.ComponentModel.DataAnnotations;

namespace QuizApi.Dtos.User
{
    public class RegisterUserDTO
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email cannot be empty")]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [MinLength(6, ErrorMessage = "Password has to be minmum 6 chars long")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password cannot be empty")]
        public string Password { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name cannot be empty")]

        public string Name { get; set; }
    }
}
