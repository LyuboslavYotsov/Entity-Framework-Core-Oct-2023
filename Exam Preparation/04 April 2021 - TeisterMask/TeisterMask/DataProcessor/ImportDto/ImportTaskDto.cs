using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using TeisterMask.Data.Models.Enums;

namespace TeisterMask.DataProcessor.ImportDto
{
    [XmlType(nameof(Task))]
    public class ImportTaskDto
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; } = null!;

        [Required]
        public string OpenDate { get; set; } = null!;

        [Required]
        public string DueDate { get; set; } = null!;

        [Required]
        [Range(0, 3)]
        public int ExecutionType { get; set; }

        [Required]
        [Range(0, 4)]
        public int LabelType { get; set; }
    }
}
