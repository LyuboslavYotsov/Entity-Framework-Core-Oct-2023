namespace VaporStore.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.Data.Models.Enums;
    using VaporStore.DataProcessor.ImportDto;
    using VaporStore.Extensions;

    public static class Deserializer
    {
        public const string ErrorMessage = "Invalid Data";

        public const string SuccessfullyImportedGame = "Added {0} ({1}) with {2} tags";

        public const string SuccessfullyImportedUser = "Imported {0} with {1} cards";

        public const string SuccessfullyImportedPurchase = "Imported {0} for {1}";

        public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            StringBuilder result = new StringBuilder();

            ImportGameDto[]? gamesDtos = JsonConvert.DeserializeObject<ImportGameDto[]>(jsonString);

            ICollection<Game> validGames = new HashSet<Game>();


            foreach (var gameDto in gamesDtos)
            {
                DateTime.TryParseExact(gameDto.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime gameDate);

                if (!IsValid(gameDto) || gameDate == null || gameDate == DateTime.MinValue || gameDto.Tags.Length == 0)
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                Developer? newDev = context.Developers.FirstOrDefault(d => d.Name == gameDto.Developer);
                Genre? newGenre = context.Genres.FirstOrDefault(g => g.Name == gameDto.Genre);

                Game newGame = new Game()
                {
                    Name = gameDto.Name,
                    Price = gameDto.Price,
                    ReleaseDate = gameDate
                };

                if (newDev == null)
                {
                    newGame.Developer = new Developer()
                    {
                        Name = gameDto.Developer,
                    };
                }
                else
                {
                    newGame.DeveloperId = newDev.Id;
                }

                if (newGenre == null)
                {
                    newGame.Genre = new Genre()
                    {
                        Name = gameDto.Genre
                    };
                }
                else
                {
                    newGame.GenreId = newGenre.Id;
                }

                foreach (var tagImport in gameDto.Tags.Distinct())
                {
                    Tag? tag = context.Tags.FirstOrDefault(t => t.Name == tagImport);

                    if (tag == null)
                    {
                        Tag newTag = new Tag()
                        {
                            Name = tagImport
                        };

                        newGame.GameTags.Add(new GameTag()
                        {
                            Game = newGame,
                            Tag = newTag
                        });
                    }
                    else
                    {
                        newGame.GameTags.Add(new GameTag()
                        {
                            Game = newGame,
                            Tag = tag
                        });
                    }
                }

                context.Games.Add(newGame);

                context.SaveChanges();

                result.AppendLine(string.Format(SuccessfullyImportedGame, newGame.Name, newGame.Genre.Name, newGame.GameTags.Count));
            }


            return result.ToString();
        }

        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
        {
            StringBuilder result = new StringBuilder();

            ImportUserDto[]? usersDtos = JsonConvert.DeserializeObject<ImportUserDto[]>(jsonString);

            string[] validCardTypes = Enum.GetNames(typeof(CardType));

            ICollection<User> validUsers = new HashSet<User>();

            foreach (var userDto in usersDtos)
            {
                if (!IsValid(userDto) || userDto.Cards.Any(c => !IsValid(c)) || userDto.Cards.Any(c => !validCardTypes.Contains(c.Type)))
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                User newUser = new User()
                {
                    FullName = userDto.FullName,
                    Username = userDto.Username,
                    Email = userDto.Email,
                    Age = userDto.Age
                };

                foreach (var cardDto in userDto.Cards)
                {
                    Card newCard = new Card()
                    {
                        Number = cardDto.Number,
                        Cvc = cardDto.Cvc,
                        Type = (CardType)Enum.Parse(typeof(CardType), cardDto.Type)
                    };

                    newUser.Cards.Add(newCard);
                }

                result.AppendLine(string.Format(SuccessfullyImportedUser, newUser.Username, newUser.Cards.Count));
                validUsers.Add(newUser);
            }

            context.Users.AddRange(validUsers);

            context.SaveChanges();

            return result.ToString();
        }

        public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
        {
            StringBuilder result = new StringBuilder();

            ImportPurchaseDto[]? purchasesDtos = xmlString.DeserializeXml<ImportPurchaseDto[]>("Purchases");

            ICollection<Purchase> validPurchases = new HashSet<Purchase>();

            foreach (var purchaseDto in purchasesDtos)
            {
                string[] validPurchaseTypes = Enum.GetNames(typeof(PurchaseType));

                Card? purchaseCard = context.Cards.FirstOrDefault(c => c.Number == purchaseDto.CardNumber);
                Game? purchaseGame = context.Games.FirstOrDefault(g => g.Name == purchaseDto.GameName);
                bool isDateValid = DateTime.TryParseExact(purchaseDto.Date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime pDate);

                if (!IsValid(purchaseDto) || purchaseCard == null || purchaseGame == null || !isDateValid || !validPurchaseTypes.Contains(purchaseDto.PurchaseType))
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                Purchase newPurchase = new Purchase()
                {
                    Type = Enum.Parse<PurchaseType>(purchaseDto.PurchaseType),
                    ProductKey = purchaseDto.ProductKey,
                    Date = pDate,
                    Card = purchaseCard,
                    Game = purchaseGame
                };

                validPurchases.Add(newPurchase);
                result.AppendLine(string.Format(SuccessfullyImportedPurchase, newPurchase.Game.Name, newPurchase.Card.User.Username));
            }

            context.Purchases.AddRange(validPurchases);
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