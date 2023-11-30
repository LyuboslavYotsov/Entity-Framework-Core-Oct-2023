using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VaporStore.Data.Models.Enums;

namespace VaporStore.Data.Models
{
    public class Card
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{4}\s[0-9]{4}\s[0-9]{4}\s[0-9]{4}$")]
        public string Number { get; set; } = null!;

        [Required]
        [RegularExpression(@"^[0-9]{3}$")]
        public string Cvc { get; set; } = null!;

        [Required]
        [Range(0, 1)]
        public CardType Type { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        public virtual ICollection<Purchase> Purchases { get; set; } = new HashSet<Purchase>();
    }
}
