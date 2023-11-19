using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;

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

            //Query1
            //string usersJson = File.ReadAllText(@"../../../Datasets/users.json");
            //Console.WriteLine(ImportUsers(context, usersJson));

            //Query2
            //string productsJson = File.ReadAllText(@"../../../Datasets/products.json");
            //Console.WriteLine(ImportProducts(context, productsJson));

            //Query3
            //string categoriesJson = File.ReadAllText(@"../../../Datasets/categories.json");
            //Console.WriteLine(ImportCategories(context, categoriesJson));

            //Query4
            //string categoriesProductsJson = File.ReadAllText(@"../../../Datasets/categories-products.json");
            //Console.WriteLine(ImportCategoryProducts(context, categoriesProductsJson));


            //Query5
            //Console.WriteLine(GetProductsInRange(context));

            //Query6
            //Console.WriteLine(GetSoldProducts(context));

            //Query7
            //Console.WriteLine(GetCategoriesByProductsCount(context));


            //Query8
            Console.WriteLine(GetUsersWithProducts(context));
        }


        //Query 1
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            ImportUserDto[] users = JsonConvert.DeserializeObject<ImportUserDto[]>(inputJson);

            foreach (var userImport in users)
            {
                User newUser = _mapper.Map<User>(userImport);

                context.Users.Add(newUser);
            }

            context.SaveChanges();

            return $"Successfully imported {users.Count()}";
        }


        //Query2
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            ImportProductDto[] importProducts = JsonConvert.DeserializeObject<ImportProductDto[]>(inputJson);

            foreach (var productImport in importProducts)
            {
                Product newProduct = _mapper.Map<Product>(productImport);
                context.Products.Add(newProduct);
            }

            context.SaveChanges();

            return $"Successfully imported {importProducts.Count()}";
        }


        //Query3
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            ImportCategoryDto[] importCategories = JsonConvert.DeserializeObject<ImportCategoryDto[]>(inputJson);


            ICollection<Category> validCategories = new HashSet<Category>();

            foreach (var categoryImport in importCategories)
            {
                if (string.IsNullOrEmpty(categoryImport.Name))
                {
                    continue;
                }

                Category newCategory = _mapper.Map<Category>(categoryImport);
                validCategories.Add(newCategory);
            }

            context.AddRange(validCategories);
            context.SaveChanges();

            return $"Successfully imported {validCategories.Count()}";
        }


        //Query4
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            ImportCategoryProductDto[] categoryProductsImports
                = JsonConvert.DeserializeObject<ImportCategoryProductDto[]>(inputJson);

            ICollection<CategoryProduct> validCategoryProducts = new List<CategoryProduct>();

            foreach (var categoryProductImport in categoryProductsImports)
            {
                CategoryProduct newProduct = _mapper.Map<CategoryProduct>(categoryProductImport);

                validCategoryProducts.Add(newProduct);
            }

            context.AddRange(validCategoryProducts);
            context.SaveChanges();

            return $"Successfully imported {validCategoryProducts.Count}";
        }


        //Query5
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .AsNoTracking()
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .ProjectTo<ExportProductsDto>(_mapper.ConfigurationProvider)
                .ToList();

            string productsJson = JsonConvert.SerializeObject(products, Formatting.Indented);

            //File.WriteAllText(@"../../../Results/products-in-range.json", productsJson);

            return productsJson;
        }


        //Query6
        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .Select(u => new
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SoldProducts = u.ProductsSold
                            .Select(ps => new
                            {
                                Name = ps.Name,
                                Price = ps.Price,
                                BuyerFirstName = ps.Buyer.FirstName,
                                BuyerLastName = ps.Buyer.LastName,
                            }).ToArray()
                })
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToArray();

            string usersJson = JsonConvert.SerializeObject(users, Formatting.Indented,
                                new JsonSerializerSettings
                                {
                                    ContractResolver = new DefaultContractResolver
                                    {
                                        NamingStrategy = new CamelCaseNamingStrategy()
                                    }
                                });

            //File.WriteAllText(@"../../../Results/users-sold-products.json", usersJson);

            return usersJson;
        }

        //Query7
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .OrderByDescending(c => c.CategoriesProducts.Count())
                .ProjectTo<ExportProductsWithAveragePriceAndTotalRevenueDto>(_mapper.ConfigurationProvider)
                .ToArray();

            string categoriesJson = JsonConvert.SerializeObject(categories, Formatting.Indented);

            //File.WriteAllText(@"../../../Results/categories-by-products.json", categoriesJson);

            return categoriesJson;
        }


        //Query8
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Any(ps => ps.Buyer != null))
                .Select(u => new
                {
                    u.FirstName,
                    u.LastName,
                    u.Age,
                    SoldProducts = new
                    {
                        Count = u.ProductsSold.Count(pb => pb.Buyer != null),
                        Products = u.ProductsSold
                            .Where(p => p.Buyer != null)
                            .Select(p => new
                            {
                                p.Name,
                                p.Price
                            }).ToArray()
                    }
                })
                .OrderByDescending(u => u.SoldProducts.Count)
                .AsNoTracking()
                .ToArray();

            var usersWrapper = new
            {
                UsersCount = users.Length,
                Users = users
            };

            string usersWithProductsJson = JsonConvert.SerializeObject(usersWrapper, Formatting.Indented,
                                new JsonSerializerSettings
                                {
                                    ContractResolver = new DefaultContractResolver
                                    {
                                        NamingStrategy = new CamelCaseNamingStrategy()
                                    },
                                    NullValueHandling = NullValueHandling.Ignore
                                    
                                });


            return usersWithProductsJson;
        }
    }
}