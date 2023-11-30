using Invoices.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoices.DataProcessor.ImportDto
{
    public class ImportProductDto
    {
        [Required]
        [MaxLength(30)]
        [MinLength(9)]
        public string Name { get; set; } = null!;

        [Required]
        [Range(5.0, 1000.0)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, 4)]
        public CategoryType CategoryType { get; set; }

        [Required]
        public int[] Clients { get; set; } = null!;
    }
}
