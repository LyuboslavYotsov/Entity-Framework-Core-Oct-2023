using Footballers.Data.Models;
using Footballers.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Xml.Serialization;

namespace Footballers.DataProcessor.ImportDto
{
    [XmlType(nameof(Footballer))]
    public class ImportFootballerDto
    {
        [Required]
        [MaxLength(40)]
        [MinLength(2)]
        public string Name { get; set; } = null!;

        [Required]
        public string ContractStartDate { get; set; } = null!;

        [Required]
        public string ContractEndDate { get; set; } = null!;

        [Required]
        [Range(0, 3)]
        public int PositionType { get; set; }

        [Required]
        [Range(0, 4)]
        public int BestSkillType { get; set; }
    }
}
