using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SoftJail.DataProcessor.ImportDto
{
    public class ImportPrisonerDto
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string FullName { get; set; } = null!;

        [Required]
        [RegularExpression(@"^The\s[A-Z]{1}[a-zA-Z]+$")]
        public string Nickname { get; set; } = null!;

        [Required]
        [Range(18, 65)]
        public int Age { get; set; }

        [Required]
        public string IncarcerationDate { get; set; } = null!;

        [JsonConverter(typeof(DateTime?))]
        public string? ReleaseDate { get; set; }

        [Range(0, (double)decimal.MaxValue)]
        public decimal? Bail { get; set; }

        [Required]
        public int? CellId { get; set; }

        public ImportMailDto[] Mails { get; set; } = null!;
    }
}
