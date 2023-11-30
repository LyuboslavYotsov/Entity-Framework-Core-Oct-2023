using SoftJail.Data.Models;
using SoftJail.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace SoftJail.DataProcessor.ImportDto
{
    [XmlType(nameof(Officer))]
    public class ImportOfficerDto
    {
        [Required]
        [StringLength(30, MinimumLength = 3)]
        [XmlElement("Name")]
        public string FullName { get; set; } = null!;

        [Required]
        [Range(0, (double)decimal.MaxValue)]
        [XmlElement("Money")]
        public decimal Salary { get; set; }

        [Required]
        public string Position { get; set; } = null!;

        [Required]
        public string Weapon { get; set; } = null!;

        [Required]
        public int DepartmentId { get; set; }

        [XmlArray("Prisoners")]
        public ImportPrisonerIdDto[] Prisoners { get; set; } = null!;
    }
}
