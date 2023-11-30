using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using TeisterMask.Data.Models;

namespace TeisterMask.DataProcessor.ImportDto
{
    [XmlType(nameof(Project))]
    public class ImportProjectDto
    {
        [Required]
        [StringLength(40, MinimumLength = 2)]
        public string Name { get; set; } = null!;

        [Required]
        public string OpenDate { get; set; } = null!;

        public string? DueDate { get; set; }

        [XmlArray("Tasks")]
        public ImportTaskDto[] Tasks { get; set; } = null!;
    }
}
