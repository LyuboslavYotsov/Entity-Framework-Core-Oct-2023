using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Theatre.DataProcessor.ImportDto
{
    [XmlType("Cast")]
    public class ImportCastDto
    {
        [Required]
        [MinLength(4)]
        [MaxLength(30)]
        public string FullName { get; set; } = null!;

        [Required]
        public bool IsMainCharacter { get; set; }

        [Required]
        [RegularExpression(@"^\+44\-[\d]{2}\-[\d]{3}\-[\d]{4}$")]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        public int PlayId { get; set; }
    }
}
