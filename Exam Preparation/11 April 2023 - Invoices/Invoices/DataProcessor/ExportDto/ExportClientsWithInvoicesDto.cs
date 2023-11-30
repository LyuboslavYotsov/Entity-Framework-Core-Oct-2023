using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Invoices.DataProcessor.ExportDto
{
    [XmlType("Client")]
    public class ExportClientsWithInvoicesDto
    {
        [XmlAttribute]
        public int InvoicesCount { get; set; }

        public string ClientName { get; set; } = null!;

        public string VatNumber { get; set; } = null!;

        [XmlArray("Invoices")]
        public ExportInvoiceDto[] Invoices { get; set; } = null!;
    }
}
