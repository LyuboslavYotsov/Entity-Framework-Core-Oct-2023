using System.ComponentModel.DataAnnotations;
using VaporStore.Data.Models.Enums;

namespace VaporStore.DataProcessor.ImportDto
{
    public class ImportCardDto
    {
        [Required]
        [RegularExpression(@"^[0-9]{4}\s[0-9]{4}\s[0-9]{4}\s[0-9]{4}$")]
        public string Number { get; set; } = null!;

        [Required]
        [RegularExpression(@"^[0-9]{3}$")]
        public string Cvc { get; set; } = null!;

        [Required]
        public string Type { get; set; } = null!;
    }
}
