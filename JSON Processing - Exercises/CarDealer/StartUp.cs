using AutoMapper;
using CarDealer.Data;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Castle.Core.Resource;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.IO;

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
            using CarDealerContext context = new CarDealerContext();

            //Query 9
            //string suppliersJson = File.ReadAllText(@"../../../Datasets/suppliers.json");
            //Console.WriteLine(ImportSuppliers(context, suppliersJson));

            //Query 10
            //string partsJson = File.ReadAllText(@"../../../Datasets/parts.json");
            //Console.WriteLine(ImportParts(context, partsJson));

            //Query 11
            //string carsJson = File.ReadAllText(@"../../../Datasets/cars.json");
            //Console.WriteLine(ImportCars(context, carsJson));

            //Query 12
            //string customersJson = File.ReadAllText(@"../../../Datasets/customers.json");
            //Console.WriteLine(ImportCustomers(context, customersJson));

            //Query 13
            //string salesJson = File.ReadAllText(@"../../../Datasets/sales.json");
            //Console.WriteLine(ImportSales(context, salesJson));

            //Query 14
            //Console.WriteLine(GetOrderedCustomers(context));

            //Query 15
            //Console.WriteLine(GetCarsFromMakeToyota(context));

            //Query 16
            //Console.WriteLine(GetLocalSuppliers(context));

            //Query 17
            //Console.WriteLine(GetCarsWithTheirListOfParts(context));

            //Query 18
            Console.WriteLine(GetTotalSalesByCustomer(context));

            //Query 19
            //Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }   

        //Query 9
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            ImportSupplierDto[] suppliers = JsonConvert.DeserializeObject<ImportSupplierDto[]>(inputJson);

            foreach (var supplierImport in suppliers)
            {
                var newSupplier = _mapper.Map<Supplier>(supplierImport);

                context.Suppliers.Add(newSupplier);
            }

            context.SaveChanges();
            return $"Successfully imported {suppliers.Length}.";
        }

        //Query 10
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            ImportPartDto[] importParts = JsonConvert.DeserializeObject<ImportPartDto[]>(inputJson);

            ICollection<Part> newParts = new HashSet<Part>();

            foreach (var importPart in importParts)
            {
                Part newPart = _mapper.Map<Part>(importPart);

                var supplier = context.Suppliers.Find(importPart.SupplierId);

                if (supplier != null)
                {
                    newParts.Add(newPart);
                }
            }

            context.Parts.AddRange(newParts);
            context.SaveChanges();

            return $"Successfully imported {newParts.Count}.";
        }

        //Query 11
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var importCars = JsonConvert.DeserializeObject<ImportCarDto[]>(inputJson);


            ICollection<Car> newCars = new HashSet<Car>();
            ICollection<PartCar> newCarParts = new HashSet<PartCar>();

            foreach (var carImport in importCars)
            {
                var newCar = new Car()
                {
                    Make = carImport.Make,
                    Model = carImport.Model,
                    TraveledDistance = carImport.TraveledDistance
                };

                foreach (var partId in carImport.PartsId.Distinct())
                {
                    var newCarPart = new PartCar()
                    {
                        PartId = partId,
                        Car = newCar
                    };

                    newCarParts.Add(newCarPart);

                }

                newCars.Add(newCar);
            }

            context.Cars.AddRange(newCars);
            context.PartsCars.AddRange(newCarParts);

            context.SaveChanges();

            return $"Successfully imported {newCars.Count}.";
        }


        //Query 12
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customersImport = JsonConvert.DeserializeObject<ImportCustomerDto[]>(inputJson);

            ICollection<Customer> newCustomers = new HashSet<Customer>();

            foreach (var customerImport in customersImport)
            {
                Customer newCustomer = new Customer()
                {
                    Name = customerImport.Name,
                    BirthDate = customerImport.BirthDate,
                    IsYoungDriver = customerImport.IsYoungDriver
                };

                newCustomers.Add(newCustomer);
            }

            context.AddRange(newCustomers);
            context.SaveChanges();


            return $"Successfully imported {newCustomers.Count}.";
        }


        //Query 13
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            ImportSaleDto[] importSales = JsonConvert.DeserializeObject<ImportSaleDto[]>(inputJson);

            ICollection<Sale> newSales = new HashSet<Sale>();

            foreach (var importSale in importSales)
            {
                Sale newSale = _mapper.Map<Sale>(importSale);

                newSales.Add(newSale);
            }

            context.AddRange(newSales);
            context.SaveChanges() ;

            return $"Successfully imported {newSales.Count}.";
        }


        //Query 14
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .AsNoTracking()
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new ExportCustomerDto()
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy"),
                    IsYoungDriver = c.IsYoungDriver
                })
                .ToArray();

            string customersJson = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return customersJson;
        }

        //Query 15
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var toyotaCars = context.Cars
                .AsNoTracking()
                .Where(c => c.Make == "Toyota")
                .Select(c => new ExportCarDto()
                {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance
                })
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .ToArray();

            string carsJson = JsonConvert.SerializeObject(toyotaCars, Formatting.Indented);

            return carsJson;
        }


        //Query 16
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var localSuppliers = context.Suppliers
                .AsNoTracking()
                .Where(s => s.IsImporter == false)
                .Select (s => new ExportSupplierDto()
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToArray();

            string suppliersJson = JsonConvert.SerializeObject(localSuppliers, Formatting.Indented);

            return suppliersJson;
        }


        //Query 17
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .AsNoTracking()
                .Select(c => new ExportCarWithPartsDto()
                {
                    Car = new ExportCarDto2()
                    {
                        Make = c.Make,
                        Model   = c.Model,
                        TraveledDistance = c.TraveledDistance,
                    },

                    Parts = c.PartsCars.Select(pc => new ExportPartDto()
                    {
                        Name = pc.Part.Name,
                        Price = pc.Part.Price.ToString("f2")
                    })
                    .ToArray()
                })
                .ToArray();

            string carsJson = JsonConvert.SerializeObject(cars, Formatting.Indented);

            //File.WriteAllText(@"../../../Results/carsExport.json", carsJson);

            return carsJson;
        }


        //Query 18
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customersWithCars = context.Customers
                .Where(c => c.Sales.Count() > 0)
                .Select(c => new
                {
                    fullName = c.Name,
                    boughtCars = c.Sales.Count(),
                    spentMoney = c.Sales.SelectMany(s => s.Car.PartsCars).Sum(pc => pc.Part.Price)
                })
                .OrderByDescending(s => s.spentMoney)
                .ThenByDescending(s => s.boughtCars)
                .ToArray();

            string resultJson = JsonConvert.SerializeObject(customersWithCars, Formatting.Indented);
            return resultJson;
        }


        //Query 19
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Take(10)
                .Select(s => new
                {
                    car = new
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TraveledDistance = s.Car.TraveledDistance
                    },

                    customerName = s.Customer.Name,
                    discount = s.Discount.ToString("f2"),
                    price = s.Car.PartsCars.Sum(pc => pc.Part.Price).ToString("f2"),
                    priceWithDiscount = (s.Car.PartsCars.Sum(pc => pc.Part.Price) * (1 - s.Discount / 100)).ToString("f2")

                }).ToArray();

            string result = JsonConvert.SerializeObject(sales, Formatting.Indented);

            return result;
        }
    }   
}