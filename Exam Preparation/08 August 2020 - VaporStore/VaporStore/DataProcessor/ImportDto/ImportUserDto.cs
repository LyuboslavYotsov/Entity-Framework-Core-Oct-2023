using System.ComponentModel.DataAnnotations;

namespace VaporStore.DataProcessor.ImportDto
{
    public class ImportUserDto
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Username { get; set; } = null!;

        [Required]
        [RegularExpression(@"^[A-Z]{1}[a-z]*\s[A-Z]{1}[a-z]*$")]
        public string FullName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [Range(3, 103)]
        public int Age { get; set; }

        public ImportCardDto[] Cards { get; set; } = null!;
    }
}
