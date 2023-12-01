using System.Xml.Serialization;

namespace Invoices.DataProcessor.ExportDto
{
    [XmlType("Invoice")]
    public class ExportInvoiceDto
    {
        public int InvoiceNumber { get; set; }

        public double InvoiceAmount { get; set; }

        public string DueDate { get; set; } = null!;

        public string Currency { get; set; } = null!;
    }
}
