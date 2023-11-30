using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftJail.Data.Models
{
    public class Prisoner
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string FullName { get; set; } = null!;

        [Required]
        [RegularExpression(@"^The\s[A-Z]{1}[a-zA-Z]+$")]
        public string Nickname { get; set; } = null!;

        [Required]
        [Range(18, 65)]
        public int Age { get; set; }

        [Required]
        public DateTime IncarcerationDate { get; set; }

        public DateTime? ReleaseDate { get; set; }

        [Range(0, (double)decimal.MaxValue)]
        public decimal? Bail { get; set; }

        [Required]
        public int? CellId { get; set; }

        [ForeignKey(nameof(CellId))]
        public Cell? Cell { get; set; }

        public virtual ICollection<Mail> Mails { get; set; } = new HashSet<Mail>();
        public virtual ICollection<OfficerPrisoner> PrisonerOfficers { get; set; } = new HashSet<OfficerPrisoner>();
    }
}
