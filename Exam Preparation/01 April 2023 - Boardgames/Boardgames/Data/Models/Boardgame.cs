using Boardgames.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Boardgames.Data.Models
{
    public class Boardgame
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        [MinLength(10)]
        public string Name { get; set; } = null!;

        [Required]
        [Range(1.0, 10.0)]
        public double Rating { get; set; }

        [Required]
        [Range(2018,2023)]
        public int YearPublished { get; set; }

        [Required]
        [Range(0, 4)]
        public CategoryType CategoryType { get; set; }

        [Required]
        public string Mechanics { get; set; } = null!;

        [Required]
        public int CreatorId { get; set; }

        [ForeignKey(nameof(CreatorId))]
        public Creator Creator { get; set; } = null!;

        public virtual ICollection<BoardgameSeller> BoardgamesSellers { get; set; } = new List<BoardgameSeller>();
    }
}
