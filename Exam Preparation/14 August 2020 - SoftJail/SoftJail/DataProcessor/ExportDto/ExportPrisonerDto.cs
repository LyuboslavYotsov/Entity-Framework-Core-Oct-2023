using SoftJail.Data.Models;
using System.Xml.Serialization;

namespace SoftJail.DataProcessor.ExportDto
{
    [XmlType(nameof(Prisoner))]
    public class ExportPrisonerDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string IncarcerationDate { get; set; } = null!;

        [XmlArray("EncryptedMessages")]
        public ExportMessageDto[] EncryptedMessages { get; set; } = null!;
    }
}
