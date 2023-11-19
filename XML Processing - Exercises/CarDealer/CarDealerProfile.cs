using AutoMapper;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            CreateMap<ImportSupplierDto, Supplier>();

            CreateMap<ImportPartDto, Part>();

            CreateMap<ImportCarDto, Car>();

            CreateMap<ImportCustomerDto, Customer>();

            CreateMap<ImportSaleDto, Sale>();

            
            CreateMap<Car, ExportCarsWithDistanceDto>();

            CreateMap<Car, ExportBMWCarsDto>();

            CreateMap<Supplier, ExportLocalSuppliersDto>()
                .ForMember(es => es.PartsCount, s => s.MapFrom(x => x.Parts.Count()));

            CreateMap<Part, ExportPartWithPriceDto>();

            CreateMap<Car, ExportCarWithAttributesDto>();

            CreateMap<Car, ExportCarsWithPartsDto>()
                .ForMember(ec => ec.Parts, c => c.MapFrom(x => x.PartsCars.Select(pc => pc.Part).OrderByDescending(p => p.Price).ToArray()));

            CreateMap<Customer, ExportTotalSalesByCustomer>()
                .ForMember(ec => ec.FullName, c => c.MapFrom(x => x.Name))
                .ForMember(ec => ec.BoughtCars, c => c.MapFrom(x => x.Sales.Count(s => s.Car != null)))
                .ForMember(ec => ec.SpentMoney, c => c.MapFrom(x => x.Sales.SelectMany(s => s.Car.PartsCars).Sum(pc => pc.Part.Price)));

            CreateMap<decimal, double>().ConvertUsing(x => (double)x);
            CreateMap<decimal, int>().ConvertUsing(x => (int)x);

            CreateMap<Sale, ExportSaleWithDiscountDto>()
                .ForMember(es => es.CustomerName, s => s.MapFrom(x => x.Customer.Name))
                .ForMember(es => es.Price, s => s.MapFrom(x => x.Car.PartsCars.Sum(pc => pc.Part.Price)))
                .ForMember(es => es.PriceWithDiscount, s => s.MapFrom(x => x.Car.PartsCars.Sum(pc => pc.Part.Price) * (1 - x.Discount / 100)));
        }
    }
}
