using System.ComponentModel.DataAnnotations;

namespace Artillery.Data.Models
{
    public class Shell
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(2.0, 1680.0)]
        public double ShellWeight { get; set; }

        [Required]
        [MaxLength(30)]
        [MinLength(4)]
        public string Caliber { get; set; } = null!;

        public virtual ICollection<Gun> Guns { get; set; } = new List<Gun>();
    }
}
