using Boardgames.Data.Models;
using System.Xml.Serialization;

namespace Boardgames.DataProcessor.ExportDto
{
    [XmlType(nameof(Boardgame))]
    public class ExportBoardgameXmlDto
    {
        public string BoardgameName { get; set; } = null!;

        public int BoardgameYearPublished { get; set; }
    }
}
