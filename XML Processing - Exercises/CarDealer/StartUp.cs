using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarDealer.Data;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Microsoft.EntityFrameworkCore;
using ProductShop;

namespace CarDealer
{
    public class StartUp
    {
        static IMapper _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CarDealerProfile>();
        }));

        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();

            //Query 9
            //string suppliersXml = File.ReadAllText("../../../Datasets/suppliers.xml");
            //Console.WriteLine(ImportSuppliers(context, suppliersXml));

            //Query 10
            //string partsXml = File.ReadAllText("../../../Datasets/parts.xml");
            //Console.WriteLine(ImportParts(context, partsXml));

            //Query 11
            //string carsXml = File.ReadAllText("../../../Datasets/cars.xml");
            //Console.WriteLine(ImportCars(context, carsXml));

            //Query 12
            //string customersXml = File.ReadAllText("../../../Datasets/customers.xml");
            //Console.WriteLine(ImportCustomers(context, customersXml));

            //Query 13
            //string salesXml = File.ReadAllText("../../../Datasets/sales.xml");
            //Console.WriteLine(ImportSales(context, salesXml));

            //Query 14
            //Console.WriteLine(GetCarsWithDistance(context));

            //Query 15
            //Console.WriteLine(GetCarsFromMakeBmw(context));

            //Query 16
            //Console.WriteLine(GetLocalSuppliers(context));

            //Query 17
            //Console.WriteLine(GetCarsWithTheirListOfParts(context));

            //Query 18
            //Console.WriteLine(GetTotalSalesByCustomer(context));

            //Query 19
            Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }


        //Query 9
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            ImportSupplierDto[] suppliersDtos = XmlHelper.DeserializeXml<ImportSupplierDto[]>(inputXml, "Suppliers");

            ICollection<Supplier> newSuppliers = _mapper.Map<Supplier[]>(suppliersDtos);

            context.Suppliers.AddRange(newSuppliers);
            context.SaveChanges();

            return $"Successfully imported {newSuppliers.Count}";
        }


        //Query 10
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            int[] validSupplierIds = context.Suppliers.Select(x => x.Id).ToArray();

            ImportPartDto[] importParts = XmlHelper.DeserializeXml<ImportPartDto[]>(inputXml, "Parts");

            Part[] newParts = importParts
                .Where(ip => validSupplierIds.Contains(ip.SupplierId))
                .Select(ip => _mapper.Map<Part>(ip))
                .ToArray();

            context.Parts.AddRange(newParts);
            context.SaveChanges();
            return $"Successfully imported {newParts.Length}";
        }


        //Query 11
        public static string ImportCars(CarDealerContext context, string inputXml)
        {

            ImportCarDto[] importCars = XmlHelper.DeserializeXml<ImportCarDto[]>(inputXml, "Cars");

            ICollection<Car> newCars = new List<Car>();

            foreach (var import in importCars)
            {
                Car newCar = _mapper.Map<Car>(import);

                foreach (var id in import.Parts.Select(p => p.Id).Distinct())
                {
                    newCar.PartsCars.Add(new PartCar()
                    {
                        Car = newCar,
                        PartId = id
                    });
                }

                newCars.Add(newCar);
            }

            context.Cars.AddRange(newCars);
            context.SaveChanges();

            return $"Successfully imported {newCars.Count}";
        }


        //Query 12
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            ImportCustomerDto[] importCustomers = XmlHelper.DeserializeXml<ImportCustomerDto[]>(inputXml, "Customers");

            Customer[] newCustomers = _mapper.Map<Customer[]>(importCustomers);

            context.Customers.AddRange(newCustomers);
            context.SaveChanges();

            return $"Successfully imported {newCustomers.Length}";
        }


        //Query 13
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            ImportSaleDto[] imprtSales = XmlHelper.DeserializeXml<ImportSaleDto[]>(inputXml, "Sales");

            int[] validCars = context.Cars.Select(c => c.Id).ToArray();

            Sale[] newSales = _mapper.Map<Sale[]>(imprtSales)
                .Where(s => validCars.Contains(s.CarId))
                .ToArray();

            context.Sales.AddRange(newSales);
            context.SaveChanges();

            return $"Successfully imported {newSales.Length}"; ;
        }


        //Query 14
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var carsWithDistance = context.Cars
                .Where(c => c.TraveledDistance > 2000000)
                .ProjectTo<ExportCarsWithDistanceDto>(_mapper.ConfigurationProvider)
                .OrderBy(c => c.Make)
                    .ThenBy(c => c.Model)
                .Take(10)
                .ToArray();

            string result = XmlHelper.SerializeObject(carsWithDistance, "cars");

            return result;
        }


        //Query 15
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var bmwCars = context.Cars
                .Where(c => c.Make.ToUpper() == "BMW")
                .ProjectTo<ExportBMWCarsDto>(_mapper.ConfigurationProvider)
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .ToArray();

            string result = XmlHelper.SerializeObject(bmwCars, "cars");
            return result;
        }


        //Query 16
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var localSuppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .ProjectTo<ExportLocalSuppliersDto>(_mapper.ConfigurationProvider)
                .ToArray();

            string result = XmlHelper.SerializeObject(localSuppliers, "suppliers");

            return result;
        }


        //Query 17
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carsWithParts = context.Cars
                .ProjectTo<ExportCarsWithPartsDto>(_mapper.ConfigurationProvider)
                .OrderByDescending(c => c.TraveledDistance)
                .ThenBy(c => c.Model)
                .Take(5)
                .ToArray();

            string result = XmlHelper.SerializeObject(carsWithParts, "cars");

            return result;
        }


        //Query 18
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var temp = context.Customers
                .Where(c => c.Sales.Any(s => s.Car != null))
                .Select(c => new
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count(),
                    SalesInfo = c.Sales.Select(s => new
                    {
                        Prices = c.IsYoungDriver
                                 ? s.Car.PartsCars.Sum(pc => Math.Round((double)pc.Part.Price * 0.95, 2))
                                 : s.Car.PartsCars.Sum(pc => Math.Round((double)pc.Part.Price, 2))
                    }).ToArray()
                })
                .ToArray();

            var customersWithDiscount = temp
                .Select(c => new ExportCustomerWithDiscount()
                {
                    FullName = c.FullName,
                    BoughtCars = c.BoughtCars,
                    SpentMoney = c.SalesInfo.Sum(x => (decimal)x.Prices)
                })
                .OrderByDescending(c => c.SpentMoney)
                .ToArray();

            string result = XmlHelper.SerializeObject(customersWithDiscount, "customers");

            return result;
        }


        //Query 19
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Where(s => s.Car != null)
                .ProjectTo<ExportSaleWithDiscountDto>(_mapper.ConfigurationProvider)
                .ToArray();

            string resultXml = XmlHelper.SerializeObject(sales, "sales");

            return resultXml;
        }
    }
}