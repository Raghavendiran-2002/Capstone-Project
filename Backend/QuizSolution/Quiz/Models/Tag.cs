using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models
{
    public class Tag
    {
        [Key]
        public string TagName { get; set; }

        public ICollection<QuizTag> QuizTags { get; set; }

      
    }
}
