using System.Xml.Serialization;

namespace TeisterMask.DataProcessor.ExportDto
{
    [XmlType(nameof(Task))]
    public class ExportTaskDto
    {
        public string Name { get; set; } = null!;

        public string Label { get; set; } = null!;
    }
}
