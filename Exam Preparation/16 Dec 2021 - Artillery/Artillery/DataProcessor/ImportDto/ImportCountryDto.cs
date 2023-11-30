using Artillery.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Artillery.DataProcessor.ImportDto
{
    [XmlType(nameof(Country))]
    public class ImportCountryDto
    {
        [Required]
        [MaxLength(60)]
        [MinLength(4)]
        public string CountryName { get; set; } = null!;

        [Required]
        [Range(50000, 10000000)]
        public int ArmySize { get; set; }
    }
}
