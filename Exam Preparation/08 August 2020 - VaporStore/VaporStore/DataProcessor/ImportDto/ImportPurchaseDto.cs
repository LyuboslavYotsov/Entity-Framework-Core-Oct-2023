using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using VaporStore.Data.Models;
using VaporStore.Data.Models.Enums;

namespace VaporStore.DataProcessor.ImportDto
{
    [XmlType(nameof(Purchase))]
    public class ImportPurchaseDto
    {
        [Required]
        [XmlAttribute("title")]
        public string GameName { get; set; } = null!;

        [Required]
        [XmlElement("Type")]
        public string PurchaseType { get; set; } = null!;

        [Required]
        [RegularExpression(@"^[A-Z0-9]{4}\-[A-Z0-9]{4}\-[A-Z0-9]{4}$")]
        [XmlElement("Key")]
        public string ProductKey { get; set; } = null!;

        [Required]
        public string Date { get; set; } = null!;

        [Required]
        [XmlElement("Card")]
        public string CardNumber { get; set; } = null!;
    }
}
