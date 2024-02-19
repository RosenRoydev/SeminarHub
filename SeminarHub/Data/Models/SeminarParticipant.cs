using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Policy;

namespace SeminarHub.Data.Models
{
    public class SeminarParticipant
    {
        [Required]
        [ForeignKey(nameof(SeminarId))]
        public int SeminarId { get; set; }
        public Seminar Seminar { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(ParticipantId))]
        public string ParticipantId { get; set; } = string.Empty;

        public IdentityUser Participant { get; set; } = null!;


    }
}


