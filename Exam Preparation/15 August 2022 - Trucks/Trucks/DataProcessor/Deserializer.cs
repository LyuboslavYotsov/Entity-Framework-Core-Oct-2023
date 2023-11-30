namespace Trucks.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using Castle.Core.Internal;
    using Data;
    using Newtonsoft.Json;
    using Trucks.Data.Models;
    using Trucks.Data.Models.Enums;
    using Trucks.DataProcessor.ImportDto;
    using Trucks.Extensions;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedDespatcher
            = "Successfully imported despatcher - {0} with {1} trucks.";

        private const string SuccessfullyImportedClient
            = "Successfully imported client - {0} with {1} trucks.";

        public static string ImportDespatcher(TrucksContext context, string xmlString)
        {
            StringBuilder result = new StringBuilder();

            ImportDespatcherDto[]? despatchersDtos = xmlString.DeserializeXml<ImportDespatcherDto[]>("Despatchers");

            ICollection<Despatcher> validDespatchers = new List<Despatcher>();

            foreach (var despatcherDto in despatchersDtos)
            {
                if (!IsValid(despatcherDto) || despatcherDto.Position.IsNullOrEmpty())
                {
                    result.AppendLine(ErrorMessage);    
                    continue;
                }

                Despatcher newDespatcher = new Despatcher()
                {
                    Name = despatcherDto.Name,
                    Position = despatcherDto.Position
                };

                foreach (var truckDto in despatcherDto.Trucks)
                {
                    if (!IsValid(truckDto))
                    {
                        result.AppendLine(ErrorMessage);
                        continue;
                    }

                    Truck newTruck = new Truck()
                    {
                        RegistrationNumber = truckDto.RegistrationNumber,
                        VinNumber = truckDto.VinNumber,
                        TankCapacity = truckDto.TankCapacity,
                        CargoCapacity = truckDto.CargoCapacity,
                        CategoryType = (CategoryType)truckDto.CategoryType,
                        MakeType = (MakeType)truckDto.MakeType,
                        DespatcherId = newDespatcher.Id
                    };

                    newDespatcher.Trucks.Add(newTruck);
                }

                validDespatchers.Add(newDespatcher);
                result.AppendLine(string.Format(SuccessfullyImportedDespatcher, newDespatcher.Name, newDespatcher.Trucks.Count));
            }

            context.Despatchers.AddRange(validDespatchers);
            context.SaveChanges();

            return result.ToString();
        }
        public static string ImportClient(TrucksContext context, string jsonString)
        {
            StringBuilder result = new StringBuilder();

            int[] validTrucksIds = context.Trucks.Select(t => t.Id).ToArray();

            ImportClientDto[]? importClientsDtos = JsonConvert.DeserializeObject<ImportClientDto[]>(jsonString);

            ICollection<Client> validClients = new List<Client>();

            foreach (var importClientDto in importClientsDtos)
            {
                if (!IsValid(importClientDto) || importClientDto.Type.ToLower() == "usual")
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                Client newClient = new Client()
                {
                    Name = importClientDto.Name,
                    Nationality = importClientDto.Nationality,
                    Type = importClientDto.Type
                };

                foreach (var truckId in importClientDto.Trucks.Distinct())
                {
                    if (validTrucksIds.Contains(truckId))
                    {
                        newClient.ClientsTrucks.Add(new ClientTruck()
                        {
                            TruckId = truckId
                        });
                    }
                    else
                    {
                        result.AppendLine(ErrorMessage);
                        continue;
                    }
                }

                validClients.Add(newClient);
                result.AppendLine(string.Format(SuccessfullyImportedClient, newClient.Name, newClient.ClientsTrucks.Count));
            }

            context.Clients.AddRange(validClients);
            context.SaveChanges();

            return result.ToString();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}