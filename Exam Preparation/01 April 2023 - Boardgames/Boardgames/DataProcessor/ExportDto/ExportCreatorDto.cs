using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Boardgames.DataProcessor.ExportDto
{
    [XmlType("Creator")]
    public class ExportCreatorDto
    {
        [XmlAttribute]
        public int BoardgamesCount { get; set; }

        public string CreatorName { get; set; } = null!;

        [XmlArray("Boardgames")]
        public ExportBoardgameXmlDto[] Boardgames { get; set; } = null!;
    }
}
