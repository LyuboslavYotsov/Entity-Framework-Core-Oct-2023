using SoftJail.Data.Models;
using System.Xml.Serialization;

namespace SoftJail.DataProcessor.ImportDto
{
    [XmlType(nameof(Prisoner))]
    public class ImportPrisonerIdDto
    {
        [XmlAttribute("id")]
        public int PrisonerId { get; set; }
    }
}
