using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Footballers.DataProcessor.ImportDto
{
    public class ImportTeamDto
    {
        [Required]
        [MaxLength(40)]
        [MinLength(3)]
        [RegularExpression(@"^[A-Za-z\s0-9\-\.]+$")]
        public string Name { get; set; } = null!;


        [Required]
        [MaxLength(40)]
        [MinLength(2)]
        public string Nationality { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue)]
        public int Trophies { get; set; }

        public int[] Footballers { get; set; } = null!;
    }
}
