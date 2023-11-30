using Artillery.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Artillery.DataProcessor.ImportDto
{
    [XmlType(nameof(Shell))]
    public class ImportShellDto
    {
        [Required]
        [Range(2.0, 1680.0)]
        public double ShellWeight { get; set; }

        [Required]
        [MaxLength(30)]
        [MinLength(4)]
        public string Caliber { get; set; } = null!;
    }
}
