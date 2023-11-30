using Invoices.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Invoices.Data.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        [MinLength(9)]
        public string Name { get; set; } = null!;

        [Required]
        [Range(5.0, 1000.0)]
        public decimal Price { get; set; }

        [Required]
        [Range(0,4)]
        public CategoryType CategoryType { get; set; }

        public virtual ICollection<ProductClient> ProductsClients { get; set; } = new List<ProductClient>();
    }
}
