using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftJail.Data.Models
{
    public class OfficerPrisoner
    {
        [Required]
        public int PrisonerId { get; set; }

        [Required]
        [ForeignKey(nameof(PrisonerId))]
        public Prisoner Prisoner { get; set; } = null!;

        [Required]
        public int OfficerId { get; set; }

        [Required]
        [ForeignKey(nameof(OfficerId))]
        public Officer Officer { get; set; } = null!;
    }
}
