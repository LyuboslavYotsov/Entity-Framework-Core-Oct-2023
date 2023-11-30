using Artillery.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Artillery.DataProcessor.ImportDto
{
    [XmlType(nameof(Manufacturer))]
    public class ImportManufacturerDto
    {
        [Required]
        [StringLength(40)]
        [MinLength(4)]
        public string ManufacturerName { get; set; } = null!;

        [Required]
        [StringLength(100)]
        [MinLength(10)]
        public string Founded { get; set; } = null!;
    }
}
