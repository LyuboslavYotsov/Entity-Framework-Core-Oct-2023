using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Trucks.Data.Models;

namespace Trucks.DataProcessor.ImportDto
{
    [XmlType(nameof(Despatcher))]
    public class ImportDespatcherDto
    {
        [Required]
        [MaxLength(40)]
        [MinLength(2)]
        public string Name { get; set; } = null!;
        [Required]
        public string Position { get; set; } = null!;

        [XmlArray("Trucks")]
        public ImportTruckDto[] Trucks { get; set; } = null!;
    }
}
