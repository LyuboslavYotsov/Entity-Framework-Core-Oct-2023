namespace Invoices.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using Invoices.Data;
    using Invoices.Data.Models;
    using Invoices.DataProcessor.ImportDto;
    using Invoices.Extensions;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedClients
            = "Successfully imported client {0}.";

        private const string SuccessfullyImportedInvoices
            = "Successfully imported invoice with number {0}.";

        private const string SuccessfullyImportedProducts
            = "Successfully imported product - {0} with {1} clients.";


        public static string ImportClients(InvoicesContext context, string xmlString)
        {
            StringBuilder result = new StringBuilder();

            ImportClientDto[]? clientsDtos = xmlString.DeserializeXml<ImportClientDto[]>("Clients");

            ICollection<Client> validClients = new HashSet<Client>();

            foreach (var clientDto in clientsDtos)
            {
                if (!IsValid(clientDto))
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                Client newClient = new Client()
                {
                    Name = clientDto.Name,
                    NumberVat = clientDto.NumberVat,
                };

                foreach (var clientAddress in clientDto.Addresses)
                {
                    if (!IsValid(clientAddress))
                    {
                        result.AppendLine(ErrorMessage);
                        continue;
                    }

                    Address newAddress = new Address()
                    {
                        StreetName = clientAddress.StreetName,
                        StreetNumber = clientAddress.StreetNumber,
                        PostCode = clientAddress.PostCode,
                        City = clientAddress.City,
                        Country = clientAddress.Country
                    };

                    newClient.Addresses.Add(newAddress);
                }

                validClients.Add(newClient);
                result.AppendLine(string.Format(SuccessfullyImportedClients, newClient.Name));
            }

            context.Clients.AddRange(validClients);

            context.SaveChanges();

            return result.ToString();
        }


        public static string ImportInvoices(InvoicesContext context, string jsonString)
        {
            StringBuilder result = new StringBuilder();

            ImportInvoiceDto[]? invoicesDtos = JsonConvert.DeserializeObject<ImportInvoiceDto[]>(jsonString);

            int[] validClientsIds = context.Clients.Select(c => c.Id).ToArray();

            ICollection<Invoice> validInvoices = new HashSet<Invoice>();

            foreach (var invoiceDto in invoicesDtos)
            {
                if (!IsValid(invoiceDto) || invoiceDto.IssueDate == DateTime.MinValue || invoiceDto.DueDate == DateTime.MinValue
                    || invoiceDto.DueDate < invoiceDto.IssueDate || !validClientsIds.Contains(invoiceDto.ClientId))
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                Invoice newInvoice = new Invoice()
                {
                    Number = invoiceDto.Number,
                    IssueDate = invoiceDto.IssueDate,
                    DueDate = invoiceDto.DueDate,
                    Amount = invoiceDto.Amount,
                    CurrencyType = invoiceDto.CurrencyType,
                    ClientId = invoiceDto.ClientId
                };

                validInvoices.Add(newInvoice);
                result.AppendLine(string.Format(SuccessfullyImportedInvoices, newInvoice.Number));
            }

            context.Invoices.AddRange(validInvoices);

            context.SaveChanges();

            return result.ToString();
        }

        public static string ImportProducts(InvoicesContext context, string jsonString)
        {
            StringBuilder result = new StringBuilder();

            ImportProductWithClientsDto[]? productsDtos = JsonConvert.DeserializeObject<ImportProductWithClientsDto[]>(jsonString);

            int[] validClientsIds = context.Clients.Select(c => c.Id).ToArray();

            ICollection<Product> validProducts = new HashSet<Product>();

            foreach (var productDto in productsDtos)
            {
                if (!IsValid(productDto))
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                Product newProduct = new Product()
                {
                    Name = productDto.Name,
                    Price = productDto.Price,
                    CategoryType = productDto.CategoryType
                };

                foreach (var clientId in productDto.ClientsIds.Distinct())
                {
                    if (!validClientsIds.Contains(clientId))
                    {
                        result.AppendLine(ErrorMessage);
                        continue;
                    }

                    newProduct.ProductsClients.Add(new ProductClient()
                    {
                        Product = newProduct,
                        ClientId = clientId
                    });
                }

                validProducts.Add(newProduct);
                result.AppendLine(string.Format(SuccessfullyImportedProducts, newProduct.Name, newProduct.ProductsClients.Count));
            }

            context.Products.AddRange(validProducts);

            context.SaveChanges();

            return result.ToString();
        }

        public static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    } 
}
