using Invoices.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Invoices.DataProcessor.ImportDto
{
    [XmlType(nameof(Address))]
    public class ImportClientAddressDto
    {
        [Required]
        [StringLength(20, MinimumLength = 10)]
        public string StreetName { get; set; } = null!;

        [Required]
        public int StreetNumber { get; set; }

        [Required]
        public string PostCode { get; set; } = null!;

        [Required]
        [StringLength(15, MinimumLength = 5)]
        public string City { get; set; } = null!;

        [Required]
        [StringLength(15, MinimumLength = 5)]
        public string Country { get; set; } = null!;
    }
}
