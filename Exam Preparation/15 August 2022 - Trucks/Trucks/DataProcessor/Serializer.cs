namespace Trucks.DataProcessor
{
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using Trucks.Data.Models.Enums;
    using Trucks.DataProcessor.ExportDto;
    using Trucks.Extensions;

    public class Serializer
    {
        public static string ExportDespatchersWithTheirTrucks(TrucksContext context)
        {
            var despatchersWithTrucks = context.Despatchers
                .AsNoTracking()
                .Where(d => d.Trucks.Count > 0)
                .Select(d => new ExportDespatcherDto()
                {
                    DespatcherName = d.Name,
                    TrucksCount = d.Trucks.Count,
                    Trucks = d.Trucks.Select(t => new ExportTruckXmlDto()
                    {
                        RegistrationNumber = t.RegistrationNumber,
                        Make = t.MakeType.ToString()
                    })
                    .OrderBy(t => t.RegistrationNumber)
                    .ToArray()
                })
                .OrderByDescending(ed => ed.TrucksCount)
                .ThenBy(ed => ed.DespatcherName)
                .ToArray();

            string result = despatchersWithTrucks.SerializeXml("Despatchers");

            return result;
        }

        public static string ExportClientsWithMostTrucks(TrucksContext context, int capacity)
        {
            var clientsWithTrucks = context.Clients
                .Where(c => c.ClientsTrucks.Any(ct => ct.Truck.TankCapacity >= capacity))
                .Select(c => new ExportClientWithTrucksDto()
                {
                    Name = c.Name,
                    Trucks = c.ClientsTrucks
                                .Where(ct => ct.Truck.TankCapacity >= capacity)
                                .Select(ct => new ExportTruckDto()
                                {
                                    TruckRegistrationNumber = ct.Truck.RegistrationNumber,
                                    VinNumber = ct.Truck.VinNumber,
                                    TankCapacity = ct.Truck.TankCapacity,
                                    CargoCapacity = ct.Truck.CargoCapacity,
                                    CategoryType = ct.Truck.CategoryType,
                                    MakeType = ct.Truck.MakeType
                                })
                                .OrderBy(t => t.MakeType)
                                .ThenByDescending(t => t.CargoCapacity)
                                .ToArray()
                })
                .OrderByDescending(ec => ec.Trucks.Length)
                .ThenBy(ec => ec.Name)
                .Take(10)
                .ToArray();
                
            string result = JsonConvert.SerializeObject(clientsWithTrucks, Formatting.Indented);

            return result;
        }
    }
}
