using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VaporStore.Data.Models
{
    public class Game
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        [Range(0, (double)decimal.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        [Required]
        public int DeveloperId { get; set; }

        [ForeignKey(nameof(DeveloperId))]
        public Developer Developer { get; set; } = null!;

        [Required]
        public int GenreId { get; set; }

        [ForeignKey(nameof(GenreId))]
        public Genre Genre { get; set; } = null!;

        public virtual ICollection<Purchase> Purchases { get; set; } = new HashSet<Purchase>();

        public virtual ICollection<GameTag> GameTags { get; set; } = new HashSet<GameTag>();
    }
}
