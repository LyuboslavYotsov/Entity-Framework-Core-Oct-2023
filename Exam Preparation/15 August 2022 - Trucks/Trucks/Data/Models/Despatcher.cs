using System.ComponentModel.DataAnnotations;

namespace Trucks.Data.Models
{
    public class Despatcher
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        [MinLength(2)]
        public string Name { get; set; } = null!;
        [Required]
        public string Position { get; set; } = null!;

        public ICollection<Truck> Trucks { get; set; } = new List<Truck>();
    }
}
