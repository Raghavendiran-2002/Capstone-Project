using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace QuizApp.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<Quiz> Quizzes { get; set; }
        public ICollection<AllowedUser> AllowedQuizzes { get; set; }
        public ICollection<Attempt> Attempts { get; set; }
        public ICollection<Certificate> Certificates { get; set; }
    }
}
