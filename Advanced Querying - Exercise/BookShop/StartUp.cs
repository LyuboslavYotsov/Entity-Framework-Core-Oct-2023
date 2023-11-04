namespace BookShop
{
    using BookShop.Models;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using System.Globalization;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            Console.WriteLine(RemoveBooks(db));
        }
        //2.	Age Restriction
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            StringBuilder result = new StringBuilder();

            var books = context.Books
                .ToArray()
                .Where(b => b.AgeRestriction.ToString().ToLower() == command.ToLower())
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToArray();

            foreach (var book in books)
            {
                result.AppendLine(book);
            }

            return result.ToString().Trim();
        }


        //3.	Golden Books
        public static string GetGoldenBooks(BookShopContext context)
        {
            StringBuilder result = new StringBuilder();

            var books = context.Books
                .AsNoTracking()
                .Where(b => (int)b.EditionType == 2 && b.Copies < 5000)
                .Select(b => b.Title);

            foreach (var book in books)
            {
                result.AppendLine(book);
            }

            return result.ToString().Trim();
        }


        //4.	Books by Price
        public static string GetBooksByPrice(BookShopContext context)
        {
            StringBuilder result = new StringBuilder();

            var books = context.Books
                .AsNoTracking()
                .Where(b => b.Price > 40m)
                .Select(b => new
                {
                    b.Title,
                    b.Price
                })
                .OrderByDescending(b => b.Price)
                .ToArray();

            foreach (var book in books)
            {
                result.AppendLine($"{book.Title} - ${book.Price:f2}");
            }

            return result.ToString().Trim();
        }



        //5.	Not Released In
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            StringBuilder result = new StringBuilder();

            var books = context.Books
                .AsNoTracking()
                .Where(b => b.ReleaseDate.Value.Year != year)
                .Select(b => b.Title)
                .ToArray();

            foreach (var book in books)
            {
                result.AppendLine(book);
            }

            return result.ToString().Trim();
        }




        //6.	Book Titles by Category
        public static string GetBooksByCategory(BookShopContext context, string input)
        {

            string[] validCategories = input
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.ToLower())
                .ToArray();

            var books = context.Books
                .AsNoTracking()
                .Where(b => b.BookCategories
                                .Any(bc => validCategories.Contains(bc.Category.Name.ToLower())))
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToArray();

            return string.Join(Environment.NewLine, books);
        }



        //7.	Released Before Date
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            StringBuilder result = new StringBuilder();

            DateTime targetDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var books = context.Books
                .AsNoTracking()
                .Where(b => b.ReleaseDate < targetDate)
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => new
                {
                    b.Title,
                    b.EditionType,
                    b.Price
                })
                .ToArray();

            foreach (var book in books)
            {
                result.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:f2}");
            }

            return result.ToString().Trim();
        }



        //8.	Author Search
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {

            var authors = context.Authors
                .AsNoTracking()
                .Where(a => a.FirstName.EndsWith(input))
                .Select(a => a.FirstName + " " + a.LastName)
                .OrderBy(a => a)
                .ToArray();



            return string.Join(Environment.NewLine, authors);
        }



        //9.	Book Search
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context.Books
                .AsNoTracking()
                .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToArray ();

            return string.Join(Environment.NewLine, books);
        }



        //10.	Book Search by Author
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context.Books
                .AsNoTracking()
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .Select(b => b.Title + " " + "(" + b.Author.FirstName + " " + b.Author.LastName + ")")
                .ToArray();

            return string.Join(Environment.NewLine,books);
        }




        //11.	Count Books
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            return context.Books
                .AsNoTracking()
                .Count(b => b.Title.Length > lengthCheck);
        }



        //12.	Total Book Copies
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authors = context.Authors
                .AsNoTracking()
                .Select(a => new
                {
                    AuthorName = a.FirstName + " " + a.LastName,
                    TotalCopies = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(b => b.TotalCopies)
                .ToArray();
            

            StringBuilder result = new StringBuilder();

            foreach (var author in authors)
            {
                result.AppendLine($"{author.AuthorName} - {author.TotalCopies}");
            }

            return result.ToString().Trim();
        }



        //13.	Profit by Category
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categoriesBooks = context.Categories
                .AsNoTracking()
                .Select(c => new 
                {
                    CategoryName = c.Name,
                    TotalProfit = c.CategoryBooks.Sum(cb => cb.Book.Price * cb.Book.Copies)
                })
                .OrderByDescending(c => c.TotalProfit)
                .ToArray();
                

            StringBuilder result = new StringBuilder();

            foreach (var category in categoriesBooks)
            {
                result.AppendLine($"{category.CategoryName} ${category.TotalProfit:f2}");
            }

            return result.ToString().Trim();
        }



        //14.	Most Recent Books
        public static string GetMostRecentBooks(BookShopContext context)
        {

            var categoriesBooks = context.Categories
                .AsNoTracking()
                .Select(c => new
                {
                    CategoryName = c.Name,
                    Top3Books = c.CategoryBooks
                                    .OrderByDescending(cb => cb.Book.ReleaseDate)
                                    .Select(cb => new
                                    {
                                        BookTitle = cb.Book.Title,
                                        BookReleaseYear = cb.Book.ReleaseDate.Value.Year,
                                    })
                                    .Take(3)
                                    .ToArray()
                })
                .OrderBy(c => c.CategoryName)
                .ToArray();

            StringBuilder result = new StringBuilder();

            foreach (var category in categoriesBooks)
            {
                result.AppendLine($"--{category.CategoryName}");

                foreach (var book in category.Top3Books)
                {
                    result.AppendLine($"{book.BookTitle} ({book.BookReleaseYear})");
                }
            }

            return result.ToString().Trim();
        }



        //15.	Increase Prices
        public static void IncreasePrices(BookShopContext context)
        {
            var booksInRange = context.Books
                .Where(b => b.ReleaseDate.Value.Year < 2010)
                .ToArray();

            foreach (var book in booksInRange)
            {
                book.Price += 5m;
            }

            context.SaveChanges();
        }



        //16.	Remove Books
        public static int RemoveBooks(BookShopContext context)
        {
            var booksInRange = context.Books
                .Where(b => b.Copies < 4200)
                .ToArray();

            foreach (var book in booksInRange)
            {
                context.Books.Remove(book);
            }

            context.SaveChanges();
            return booksInRange.Count();

        }
    }
}


