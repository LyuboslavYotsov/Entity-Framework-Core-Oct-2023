using Invoices.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Invoices.DataProcessor.ImportDto
{
    [XmlType(nameof(Client))]
    public class ImportClientDto
    {
        [Required]
        [StringLength(25, MinimumLength = 10)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(15, MinimumLength = 10)]
        public string NumberVat { get; set; } = null!;

        [XmlArray("Addresses")]
        public ImportClientAddressDto[] Addresses { get; set; } = null!;
    }
}
