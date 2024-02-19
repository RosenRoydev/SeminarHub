using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Policy;
using static SeminarHub.Data.Common.DataValidation;

namespace SeminarHub.Data.Models
{
    public class Seminar
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(SeminarTopicMaxLength)]
        public string Topic { get; set; } = string.Empty;

        [Required]
        [MaxLength(SeminarLecturerMaxLength)]
        public string Lecturer { get; set; } = string.Empty;

        [Required]
        [MaxLength(SeminarDetailsMaxLength)]
        public string Details { get;set; } = string.Empty;

        [Required]
        public string OrganizerId { get; set; } = string.Empty;

        [Required]
        public IdentityUser Organizer { get; set; } = null!;

        [Required]
        public DateTime DateAndTime { get; set; }

        [Range(SeminarDurationMinTime, SeminarDurationMaxTime)]
        public int Duration {  get; set; }

        [Required]
        [ForeignKey(nameof(CategoryId))]
        public int CategoryId { get; set; }

        [Required]
        public Category Category { get; set; } = null!;

        public IList<SeminarParticipant> SeminarsParticipants {  get; set; } = new List <SeminarParticipant>();


    }
}

