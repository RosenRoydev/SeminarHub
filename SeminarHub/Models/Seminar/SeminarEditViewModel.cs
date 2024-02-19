using SeminarHub.Models.Category;
using System.ComponentModel.DataAnnotations;
using static SeminarHub.Data.Common.DataValidation;

namespace SeminarHub.Models.Seminar
{
    public class SeminarEditViewModel
    {
        [Required]
        public int Id { get; set; }


        [Required(ErrorMessage = RequiredField)]
        [StringLength(SeminarTopicMaxLength
            , MinimumLength = SeminarTopicMinLength
            , ErrorMessage = RequiredLength)]
        public string Topic { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredField)]
        [StringLength(SeminarLecturerMaxLength
            , MinimumLength = SeminarLecturerMinLength
            , ErrorMessage = RequiredLength)]
        public string Lecturer { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredField)]
        [StringLength(SeminarDetailsMaxLength
            , MinimumLength = SeminarDetailsMinLength
            , ErrorMessage = RequiredLength)]
        public string Details { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredField)]
        public string DateAndTime { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredField)]
        [Range(SeminarDurationMinTime
            , SeminarDurationMaxTime
            , ErrorMessage = RequiredDuration)]
        public int Duration { get; set; }

        [Required(ErrorMessage = RequiredField)]
        public int CategoryId { get; set; }

        public ICollection<CategoriesTypesViewModel> Categories { get; set; } = new List<CategoriesTypesViewModel>();
    }
}
