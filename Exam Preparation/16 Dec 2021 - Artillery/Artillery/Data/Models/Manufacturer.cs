using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Artillery.Data.Models
{
    [Index(nameof(ManufacturerName), IsUnique = true)]
    public class Manufacturer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(40)]
        [MinLength(4)]
        public string ManufacturerName { get; set; } = null!;

        [Required]
        [StringLength(100)]
        [MinLength(10)]
        public string Founded { get; set;} = null!;

        public virtual ICollection<Gun> Guns { get; set; } = new List<Gun>();
    }
}
