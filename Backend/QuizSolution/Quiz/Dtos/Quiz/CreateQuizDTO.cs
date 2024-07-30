using System.ComponentModel.DataAnnotations;

namespace QuizApi.Dtos.Quiz
{
    public class CreateQuizDTO
    {
        [Required]        
        public int UserId { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Topic { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Range(1, 180)]
        public int Duration { get; set; }
        [Required]
        public bool DurationPerQuestion { get; set; }


        [Required]
        public DateTime StartTime { get; set; }

        [Required]       
        public DateTime EndTime { get; set; }
       

        [Required]
        [StringLength(50)]
        public string Type { get; set; }


        [Required]
        public List<CreateQuestionDTO> Questions { get; set; }

        public List<string> AllowedUsers { get; set; }
    }
}
