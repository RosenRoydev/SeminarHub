using System.ComponentModel.DataAnnotations;
using System.Security.Policy;
using static SeminarHub.Data.Common.DataValidation;

namespace SeminarHub.Data.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(CategoryTopicMaxLength)]
        public string Name { get; set; } = string.Empty;
        public IList<Seminar> Seminars { get; set; } = new List<Seminar>();

    }
}



