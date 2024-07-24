using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models
{
    public class AllowedUser
    {
        [Key]
        public int AllowedUserId { get; set; }

        [Required]
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

    }
}
