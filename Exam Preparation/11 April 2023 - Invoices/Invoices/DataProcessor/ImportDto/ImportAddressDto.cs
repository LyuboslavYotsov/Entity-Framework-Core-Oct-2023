using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Invoices.DataProcessor.ImportDto
{
    [XmlType("Address")]
    public class ImportAddressDto
    {
        [Required]
        [MaxLength(20)]
        [MinLength(10)]
        public string StreetName { get; set; } = null!;

        [Required]
        public int StreetNumber { get; set; }

        [Required]
        public string PostCode { get; set; } = null!;

        [Required]
        [MaxLength(15)]
        [MinLength(5)]
        public string City { get; set; } = null!;

        [Required]
        [MaxLength(15)]
        [MinLength(5)]
        public string Country { get; set; } = null!;
    }
}
