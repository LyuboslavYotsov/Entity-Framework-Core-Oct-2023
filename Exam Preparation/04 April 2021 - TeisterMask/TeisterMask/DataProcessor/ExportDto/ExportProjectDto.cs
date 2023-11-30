using System.Xml.Serialization;
using TeisterMask.Data.Models;

namespace TeisterMask.DataProcessor.ExportDto
{
    [XmlType(nameof(Project))]
    public class ExportProjectDto
    {
        [XmlAttribute]
        public int TasksCount { get; set; }

        public string ProjectName { get; set; } = null!;

        public string HasEndDate { get; set; } = null!;

        [XmlArray("Tasks")]
        public ExportTaskDto[] Tasks { get; set; } = null!;
    }
}
