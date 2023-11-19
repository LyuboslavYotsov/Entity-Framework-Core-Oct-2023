using AutoMapper;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile() 
        {


            //User
            CreateMap<ImportUserDto, User>();

            //Product
            CreateMap<ImportProductDto, Product>();

            CreateMap<Product, ExportProductsDto>()
                .ForMember(ep => ep.Seller, p => p.MapFrom(x => $"{x.Seller.FirstName} {x.Seller.LastName}"));


            //Category
            CreateMap<ImportCategoryDto, Category>();

            CreateMap<Category, ExportProductsWithAveragePriceAndTotalRevenueDto>()
                .ForMember(ep => ep.Category, c => c.MapFrom(x => x.Name))
                .ForMember(ep => ep.AveragePrice, c => c.MapFrom(x => x.CategoriesProducts
                .Average(ap => ap.Product.Price)
                .ToString("f2")))
                

                .ForMember(ep => ep.TotalRevenue, c => c.MapFrom(x => x.CategoriesProducts
                .Sum(ap => ap.Product.Price)
                .ToString("f2")))

                .ForMember(ep => ep.ProductsCount, p => p.MapFrom(x => x.CategoriesProducts.Count()));

            //Category-Product
            CreateMap<ImportCategoryProductDto, CategoryProduct>();
        }
    }
}
