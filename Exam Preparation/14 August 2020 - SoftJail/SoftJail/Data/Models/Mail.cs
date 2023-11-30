using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftJail.Data.Models
{
    public class Mail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public string Sender { get; set; } = null!;

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9\s]+str\.$")]
        public string Address { get; set; } = null!;

        [Required]
        public int PrisonerId { get; set; }

        [Required]
        [ForeignKey(nameof(PrisonerId))]
        public Prisoner Prisoner { get; set; } = null!;
    }
}
