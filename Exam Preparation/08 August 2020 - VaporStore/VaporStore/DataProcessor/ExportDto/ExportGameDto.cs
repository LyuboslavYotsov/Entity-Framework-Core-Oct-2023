using System.Xml.Serialization;

namespace VaporStore.DataProcessor.ExportDto
{
    [XmlType("Game")]
    public class ExportGameDto
    {
        [XmlAttribute("title")]
        public string GameTitle { get; set; } = null!;

        public string Genre { get; set; } = null!;

        public decimal Price { get; set; }
    }
}
