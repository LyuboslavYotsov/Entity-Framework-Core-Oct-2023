using Artillery.Data.Models;
using System.Xml.Serialization;

namespace Artillery.DataProcessor.ExportDto
{
    [XmlType(nameof(Gun))]
    public class ExportGunXmlDto
    {
        [XmlAttribute]
        public string Manufacturer { get; set; } = null!;
        [XmlAttribute]
        public string GunType { get; set; } = null!;
        [XmlAttribute]
        public int GunWeight { get; set; }
        [XmlAttribute]
        public double BarrelLength { get; set; }

        [XmlAttribute]
        public int Range { get; set; }

        [XmlArray("Countries")]
        public ExportCountryXmlDto[] Countries { get; set; } = null!;
    }
}
