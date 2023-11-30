using System.Xml.Serialization;

namespace VaporStore.DataProcessor.ExportDto
{
    [XmlType("User")]
    public class ExportUserDto
    {
        [XmlAttribute("username")]
        public string UserName { get; set; } = null!;

        [XmlArray("Purchases")]
        public ExportPurchaseDto[] Purchases { get; set; } = null!;
    }
}
