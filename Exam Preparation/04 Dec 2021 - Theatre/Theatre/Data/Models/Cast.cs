using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Theatre.Data.Models
{
    public class Cast
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(30)]
        public string FullName { get; set; } = null!;

        [Required]
        public bool IsMainCharacter { get; set; }

        [Required]
        [RegularExpression(@"^\+44\-[\d]{2}\-[\d]{3}\-[\d]{4}$")]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        public int PlayId { get; set; }

        [ForeignKey(nameof(PlayId))]
        public Play Play { get; set; } = null!;
    }
}
