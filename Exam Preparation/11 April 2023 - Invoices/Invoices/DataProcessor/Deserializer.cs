namespace Invoices.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using System.Xml;
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

            var importClientsDtos = xmlString.DeserializeXml<ImportClientDto[]>("Clients");

            ICollection<Client> validCLients = new HashSet<Client>();

            foreach (var clientDto in importClientsDtos)
            {
                if (!IsValid(clientDto))
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                Client newClient = new Client()
                {
                    Name = clientDto.Name,
                    NumberVat = clientDto.NumberVat
                };



                foreach (var addressDto in clientDto.Addresses)
                {
                    if (!IsValid(addressDto))
                    {
                        result.AppendLine(ErrorMessage);
                        continue;
                    }

                    Address newAddress = new Address()
                    {
                        StreetName = addressDto.StreetName,
                        StreetNumber = addressDto.StreetNumber,
                        PostCode = addressDto.PostCode,
                        City = addressDto.City,
                        Country = addressDto.Country,
                    };
                    newClient.Addresses.Add(newAddress);
                }
                validCLients.Add(newClient);
                result.AppendLine(string.Format(SuccessfullyImportedClients, newClient.Name));
            }

            context.Clients.AddRange(validCLients);
            context.SaveChanges();
            return result.ToString();
        }


        public static string ImportInvoices(InvoicesContext context, string jsonString)
        {
            StringBuilder result = new StringBuilder();
            ICollection<Invoice> validInvoices = new HashSet<Invoice>();

            var importInvoicesDtos = JsonConvert.DeserializeObject<ImportInvoiceDto[]>(jsonString);

            foreach (var importInvoiceDto in importInvoicesDtos)
            {
                if (!IsValid(importInvoiceDto) || importInvoiceDto.DueDate < importInvoiceDto.IssueDate
                    || importInvoiceDto.DueDate == DateTime.MinValue || importInvoiceDto.IssueDate == DateTime.MinValue)
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                Invoice newInvoice = new Invoice()
                {
                    Number = importInvoiceDto.Number,
                    IssueDate = importInvoiceDto.IssueDate,
                    DueDate = importInvoiceDto.DueDate,
                    Amount = importInvoiceDto.Amount,
                    CurrencyType = importInvoiceDto.CurrencyType,
                    ClientId = importInvoiceDto.ClientId
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
            ICollection<Product> validProducts = new HashSet<Product>();

            int[] validClients = context.Clients.Select(c => c.Id).ToArray();

            ImportProductDto[]? importProductDtos = JsonConvert.DeserializeObject<ImportProductDto[]>(jsonString);

            foreach (var importProductDto in importProductDtos)
            {
                if (!IsValid(importProductDto))
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                Product newProduct = new Product()
                {
                    Name = importProductDto.Name,
                    Price = importProductDto.Price,
                    CategoryType = importProductDto.CategoryType
                };

                foreach (var clientId in importProductDto.Clients.Distinct())
                {
                    if (!validClients.Contains(clientId))
                    {
                        result.AppendLine(ErrorMessage);
                        continue;
                    }

                    ProductClient newProductClient = new ProductClient()
                    {
                        ClientId = clientId,
                        Product = newProduct,
                    };

                    newProduct.ProductsClients.Add(newProductClient);
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
