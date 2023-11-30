using System.ComponentModel.DataAnnotations;

namespace Boardgames.Data.Models
{
    public class Seller
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        [MinLength(5)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        [MinLength(2)]
        public string Address { get; set; } = null!;

        [Required]
        public string Country { get; set; } = null!;

        [Required]
        [RegularExpression(@"^www\.[a-zA-Z\-]+\.com$")]
        public string Website { get; set; } = null!;

        public ICollection<BoardgameSeller> BoardgamesSellers { get; set; } = new List<BoardgameSeller>();
    }
}
