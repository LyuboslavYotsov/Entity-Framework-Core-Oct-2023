using Footballers.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Footballers.DataProcessor.ImportDto
{
    [XmlType(nameof(Coach))]
    public class ImportCoachDto
    {
        [Required]
        [MaxLength(40)]
        [MinLength(2)]
        public string Name { get; set; } = null!;

        [Required]
        public string Nationality { get; set; } = null!;

        [XmlArray("Footballers")]
        public ImportFootballerDto[] Footballers { get; set; } = null!;
    }
}
