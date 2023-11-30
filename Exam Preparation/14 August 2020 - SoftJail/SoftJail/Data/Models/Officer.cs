using SoftJail.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftJail.Data.Models
{
    public class Officer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string FullName { get; set; } = null!;

        [Required]
        [Range(0, (double)decimal.MaxValue)]
        public decimal Salary { get; set; }

        [Required]
        [Range (0, 3)]
        public Position Position { get; set; }

        [Required]
        [Range(0, 4)]
        public Weapon Weapon { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        [ForeignKey(nameof(DepartmentId))]
        public Department Department { get; set; } = null!;

        public virtual ICollection<OfficerPrisoner> OfficerPrisoners { get; set; } = new HashSet<OfficerPrisoner>();
    }
}
