using System.ComponentModel.DataAnnotations;

namespace TeisterMask.DataProcessor.ImportDto
{
    public class ImportEmployeeDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(40)]
        [RegularExpression(@"^[a-zA-Z\d]+$")]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [RegularExpression(@"^\d{3}\-\d{3}\-\d{4}$")]
        public string Phone { get; set; } = null!;

        public int[] Tasks { get; set; } = null!;
    }
}
