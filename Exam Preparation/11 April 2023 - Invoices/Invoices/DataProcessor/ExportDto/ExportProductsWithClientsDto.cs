using Invoices.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoices.DataProcessor.ExportDto
{
    public class ExportProductsWithClientsDto
    {
        [Required]
        [MaxLength(30)]
        [MinLength(9)]
        public string Name { get; set; } = null!;

        [Required]
        [Range(5.0, 1000.0)]
        public double Price { get; set; }

        [Required]
        [Range(0, 4)]
        public string Category { get; set; } = null!;

        [Required]
        public ExportClientNameAndVatDto[] Clients { get; set; } = null!;
    }
}
