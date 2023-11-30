using System.ComponentModel.DataAnnotations;

namespace Theatre.DataProcessor.ImportDto
{
    public class ImportTheatreDto
    {
        [Required]
        [MinLength(4)]
        [MaxLength(30)]
        public string Name { get; set; } = null!;

        [Required]
        [Range(1, 10)]
        public sbyte NumberOfHalls { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(30)]
        public string Director { get; set; } = null!;

        public ImportTicketDto[] Tickets { get; set; } = null!;
    }
}
