using System.Xml.Serialization;

namespace Invoices.DataProcessor.ExportDto
{
    [XmlType("Client")]
    public class ExportClientWithInvoicesDto
    {
        [XmlAttribute]
        public int InvoicesCount { get; set; }

        public string ClientName { get; set; } = null!;

        public string VatNumber { get; set; } = null!;

        [XmlArray]
        public ExportInvoiceDto[] Invoices { get; set; } = null!;
    }
}
