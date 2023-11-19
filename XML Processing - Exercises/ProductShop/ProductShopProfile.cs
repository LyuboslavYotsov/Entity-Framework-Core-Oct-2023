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
            CreateMap<ImportUserDto, User>();

            CreateMap<ImportProductDto, Product>();

            CreateMap<ImportCategoryDto, Category>();

            CreateMap<ImportCategoryProductDto, CategoryProduct>();

            CreateMap<Product, ExportProductDto>()
                .ForMember(ep => ep.Buyer, p => p.MapFrom(x => x.Buyer != null ? x.Buyer.FirstName + " " + x.Buyer.LastName : null));

            CreateMap<Product, ExportProductNameAndPriceDto>();

            CreateMap<User, ExportUserSoldProductsDto>()
                .ForMember(eu => eu.SoldProducts, u => u.MapFrom(x => x.ProductsSold.ToArray()));

            CreateMap<Category, ExportCategoriesByProductsDto>()
                .ForMember(ec => ec.ProductsCount, c => c.MapFrom(x => x.CategoryProducts.Count))
                .ForMember(ec => ec.ProductAveragePrice, c => c.MapFrom(x => x.CategoryProducts.Average(cp => cp.Product.Price)))
                .ForMember(ec => ec.TotalRevenue, c => c.MapFrom(x => x.CategoryProducts.Sum(cp => cp.Product.Price)));


            CreateMap<User, ExportUsersWithProductsDto>();

            CreateMap<User, ExportUsersWithProductsCountDto>()
                .ForMember(eu => eu.ProductsCount, u => u.MapFrom(x => x.ProductsSold.Count));
        }
    }
}
