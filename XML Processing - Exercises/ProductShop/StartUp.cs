using AutoMapper;
using AutoMapper.QueryableExtensions;
using Castle.Components.DictionaryAdapter;
using Microsoft.EntityFrameworkCore;
using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {

        static IMapper _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProductShopProfile>();
        }));

        public static void Main()
        {
            ProductShopContext context = new ProductShopContext();

            //Query 1
            //string usersXml = File.ReadAllText("../../../Datasets/users.xml");
            //Console.WriteLine(ImportUsers(context, usersXml));

            //Query 2
            //string productsXml = File.ReadAllText("../../../Datasets/products.xml");
            //Console.WriteLine(ImportProducts(context, productsXml));

            //Query 3
            //string categoriesXml = File.ReadAllText("../../../Datasets/categories.xml");
            //Console.WriteLine(ImportCategories(context, categoriesXml));

            //Query 4
            //string categoriesProductsXml = File.ReadAllText("../../../Datasets/categories-products.xml");
            //Console.WriteLine(ImportCategoryProducts(context, categoriesProductsXml));

            //Query 5
            //Console.WriteLine(GetProductsInRange(context));

            //Query 6
            //Console.WriteLine(GetSoldProducts(context));

            //Query 7
            //Console.WriteLine(GetCategoriesByProductsCount(context));

            //Query 8
            Console.WriteLine(GetUsersWithProducts(context));
        }


        //Query 1
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {

            ImportUserDto[] usersDtos = XmlHelper.DeserializeXml<ImportUserDto[]>(inputXml, "Users");

            var validUsers = new List<User>();

            foreach (var userDto in usersDtos)
            {
                User newUser = _mapper.Map<User>(userDto);

                validUsers.Add(newUser);
            }

            context.Users.AddRange(validUsers);
            context.SaveChanges();

            return $"Successfully imported {validUsers.Count}";
        }


        //Query 2
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            ImportProductDto[] productsDtos = XmlHelper.DeserializeXml<ImportProductDto[]>(inputXml, "Products");

            var validProducts = new List<Product>();

            foreach (var productDto in productsDtos)
            {
                Product newProduct = _mapper.Map<Product>(productDto);

                validProducts.Add(newProduct);
            }

            context.Products.AddRange(validProducts);
            context.SaveChanges();
            return $"Successfully imported {validProducts.Count}";
        }


        //Query 3
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            ImportCategoryDto[] categoryDtos = XmlHelper.DeserializeXml<ImportCategoryDto[]>(inputXml, "Categories");

            var validCategories = new List<Category>();

            foreach (var categoryDto in categoryDtos)
            {
                if (categoryDto.Name != null)
                {
                    Category newCategory = _mapper.Map<Category>(categoryDto);

                    validCategories.Add(newCategory);
                }
            }

            context.Categories.AddRange(validCategories);
            context.SaveChanges();

            return $"Successfully imported {validCategories.Count}";
        }


        //Query 4
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            ImportCategoryProductDto[] importCategoriesDtos 
                = XmlHelper.DeserializeXml<ImportCategoryProductDto[]>(inputXml, "CategoryProducts");

            var validProductsCategories = new List<CategoryProduct>();

            foreach (var productCategoryDto in importCategoriesDtos)
            {
                CategoryProduct newCategoryProduct = _mapper.Map<CategoryProduct>(productCategoryDto);

                var category = context.Categories.FirstOrDefault(p => p.Id == productCategoryDto.CategoryId);
                var product = context.Products.FirstOrDefault( p => p.Id == productCategoryDto.ProductId);

                if (category != null && product != null)
                {
                    validProductsCategories.Add(newCategoryProduct);
                }
            }

            context.CategoryProducts.AddRange(validProductsCategories);
            context.SaveChanges();

            return $"Successfully imported {validProductsCategories.Count}";
        }


        //Query 5
        public static string GetProductsInRange(ProductShopContext context)
        {
            var productsInRange = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Take(10)
                .Select(p => new ExportProductDto()
                {
                    Price = p.Price,
                    Name = p.Name,
                    Buyer = p.Buyer.FirstName + " " + p.Buyer.LastName
                })
                .ToArray();

            string resultXml = XmlHelper.SerializeObject(productsInRange, "Products");

            return resultXml;
        }


        //Query 6
        public static string GetSoldProducts(ProductShopContext context)
        {
            var userWithProducts = context.Users
                .Where(u => u.ProductsSold.Any())
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(5)
                .ProjectTo<ExportUserSoldProductsDto>(_mapper.ConfigurationProvider)
                .ToArray();

            string xmlResult = XmlHelper.SerializeObject(userWithProducts, "Users");

            return xmlResult;
        }


        //Query 7
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categoriesByProducts = context.Categories
                .Select(c => new ExportCategoriesByProductsDto()
                {
                    Name = c.Name,
                    ProductsCount = c.CategoryProducts.Count(),
                    ProductAveragePrice = c.CategoryProducts.Average(cp => cp.Product.Price),
                    TotalRevenue = c.CategoryProducts.Sum(cp => cp.Product.Price)
                })
                .OrderByDescending(c => c.ProductsCount)
                .ThenBy(c => c.TotalRevenue)
                .ToArray();


            string resultXml = XmlHelper.SerializeObject(categoriesByProducts, "Categories");

            return resultXml;
        }


        //Query 8
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Any())
                .Select(u => new ExportUsersWithProductsDto()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new ExportSoldProductsWithCount()
                    {
                        Count = u.ProductsSold.Count(),
                        Products = u.ProductsSold.Select(ps => new ExportProductNameAndPriceDto()
                        {
                            Name = ps.Name,
                            Price = ps.Price,
                        })
                        .OrderByDescending(ps => ps.Price)
                        .ToArray()
                    }
                })
                .OrderByDescending(u => u.SoldProducts.Count)
                .ToArray();

            var usersWrapper = new ExportUsersWithProductsCountDto()
            {
                Users = users.
                Select(u => u)
                .Take(10)
                .ToArray(),

                ProductsCount = users.Length
            };

            string resultXml = XmlHelper.SerializeObject(usersWrapper, "Users");

            return resultXml;
        }
    }
}