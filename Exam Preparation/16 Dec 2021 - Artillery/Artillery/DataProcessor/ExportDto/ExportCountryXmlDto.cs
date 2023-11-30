using Artillery.Data.Models;
using System.Xml.Serialization;

namespace Artillery.DataProcessor.ExportDto
{
    [XmlType(nameof(Country))]
    public class ExportCountryXmlDto
    {
        [XmlAttribute]
        public string Country { get; set; } = null!;

        [XmlAttribute]
        public int ArmySize { get; set; }
    }
}
