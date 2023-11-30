using Boardgames.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Boardgames.DataProcessor.ImportDto
{
    [XmlType(nameof(Creator))]
    public class ImportCreatorDto
    {
        [Required]
        [MaxLength(7)]
        [MinLength(2)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(7)]
        [MinLength(2)]
        public string LastName { get; set; } = null!;

        [XmlArray("Boardgames")]
        public ImportBoardGameDto[] Boardgames { get; set; } = null!;
    }
}
