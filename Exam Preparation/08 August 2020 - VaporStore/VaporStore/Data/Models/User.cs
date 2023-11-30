using System.ComponentModel.DataAnnotations;

namespace VaporStore.Data.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

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

        public virtual ICollection<Card> Cards { get; set; } = new HashSet<Card>();
    }
}
