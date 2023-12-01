namespace Invoices.DataProcessor
{
    using Invoices.Data;
    using Invoices.DataProcessor.ExportDto;
    using Invoices.Extensions;
    using Newtonsoft.Json;
    using System.Globalization;

    public class Serializer
    {
        public static string ExportClientsWithTheirInvoices(InvoicesContext context, DateTime date)
        {
            var clientsWithInvoices = context.Clients
                .Where(c => c.Invoices.Any(i => i.IssueDate > date))
                .Select(c => new ExportClientWithInvoicesDto
                {
                    InvoicesCount = c.Invoices.Count,
                    ClientName = c.Name,
                    VatNumber = c.NumberVat,
                    Invoices = c.Invoices
                                .OrderBy(i => i.IssueDate)
                                .ThenByDescending(i => i.DueDate)
                                .Select(i => new ExportInvoiceDto()
                                {
                                    InvoiceNumber = i.Number,
                                    InvoiceAmount = (double)i.Amount,
                                    DueDate = i.DueDate.ToString("d", CultureInfo.InvariantCulture),
                                    Currency = i.CurrencyType.ToString()
                                })
                                .ToArray()
                })
                .OrderByDescending(ec => ec.InvoicesCount)
                .ThenBy(ec => ec.ClientName)
                .ToArray();

            return clientsWithInvoices.SerializeXml("Clients");
        }

        public static string ExportProductsWithMostClients(InvoicesContext context, int nameLength)
        {
            var productsWithClients = context.Products
                .Where(p => p.ProductsClients.Any(pc => pc.Client.Name.Length >= nameLength))
                .Select(p => new
                {
                    Name = p.Name,
                    Price = (double)p.Price,
                    Category = p.CategoryType.ToString(),
                    Clients = p.ProductsClients
                                .Where(pc => pc.Client.Name.Length >= nameLength)
                                .Select(pc => new
                                {
                                    Name = pc.Client.Name,
                                    NumberVat = pc.Client.NumberVat
                                })
                                .OrderBy(ec => ec.Name)
                                .ToArray()
                })
                .OrderByDescending(ep => ep.Clients.Length)
                .ThenBy(ep => ep.Name)
                .Take(5)
                .ToArray();

            return JsonConvert.SerializeObject(productsWithClients, Formatting.Indented);
        }
    }
}