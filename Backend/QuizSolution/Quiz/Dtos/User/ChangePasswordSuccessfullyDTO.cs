using System.ComponentModel.DataAnnotations;

namespace QuizApi.Dtos.User
{
    public class ChangePasswordSuccessfullyDTO
    {
        [Required]
        public bool status { get; set; }

        
    }
}
