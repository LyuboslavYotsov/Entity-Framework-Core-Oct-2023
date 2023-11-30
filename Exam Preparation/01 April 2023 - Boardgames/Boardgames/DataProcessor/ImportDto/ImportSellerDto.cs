using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boardgames.DataProcessor.ImportDto
{
    public class ImportSellerDto
    {
        [Required]
        [MaxLength(20)]
        [MinLength(5)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        [MinLength(2)]
        public string Address { get; set; } = null!;

        [Required]
        public string Country { get; set; } = null!;

        [Required]
        [RegularExpression(@"^www\.[a-zA-Z\-0-9]+\.com$")]
        public string Website { get; set; } = null!;
        [Required]
        public int[] Boardgames { get; set; } = null!;
    }
}
