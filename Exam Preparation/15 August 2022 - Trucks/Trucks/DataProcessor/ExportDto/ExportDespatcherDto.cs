using System.Xml.Serialization;
using Trucks.Data.Models;

namespace Trucks.DataProcessor.ExportDto
{
    [XmlType(nameof(Despatcher))]
    public class ExportDespatcherDto
    {
        [XmlAttribute]
        public int TrucksCount { get; set; }

        public string DespatcherName { get; set; } = null!;

        [XmlArray("Trucks")]
        public ExportTruckXmlDto[] Trucks { get; set; } = null!;
    }
}
