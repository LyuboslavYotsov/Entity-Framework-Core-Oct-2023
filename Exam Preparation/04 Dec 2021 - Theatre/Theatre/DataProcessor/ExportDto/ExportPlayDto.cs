using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Theatre.Data.Models;
using Theatre.Data.Models.Enums;

namespace Theatre.DataProcessor.ExportDto
{
    [XmlType(nameof(Play))]
    public class ExportPlayDto
    {

        [XmlAttribute]
        public string Title { get; set; } = null!;

        [XmlAttribute]
        public string Duration { get; set; } = null!;

        [XmlAttribute]
        public string Rating { get; set; } = null!;

        [XmlAttribute]
        public string Genre { get; set; } = null!;

        [XmlArray("Actors")]
        public ExportActorDto[] Actors { get; set; } = null!;
    }
}
