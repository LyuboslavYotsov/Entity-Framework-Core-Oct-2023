namespace Boardgames.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using Boardgames.Data;
    using Boardgames.Data.Models;
    using Boardgames.Data.Models.Enums;
    using Boardgames.DataProcessor.ImportDto;
    using Boardgames.Extensions;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCreator
            = "Successfully imported creator – {0} {1} with {2} boardgames.";

        private const string SuccessfullyImportedSeller
            = "Successfully imported seller - {0} with {1} boardgames.";

        public static string ImportCreators(BoardgamesContext context, string xmlString)
        {
            StringBuilder result = new StringBuilder();

            ImportCreatorDto[]? importCreatorsDtos = xmlString.DeserializeXml<ImportCreatorDto[]>("Creators");

            ICollection<Creator> validCreators = new HashSet<Creator>();

            foreach (var creatorDto in importCreatorsDtos)
            {
                if (!IsValid(creatorDto))
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                Creator newCreator = new Creator()
                {
                    FirstName = creatorDto.FirstName,
                    LastName = creatorDto.LastName
                };

                foreach (var boardGameDto in creatorDto.Boardgames)
                {
                    if (!IsValid(boardGameDto))
                    {
                        result.AppendLine(ErrorMessage);
                        continue;
                    }

                    Boardgame newBg = new Boardgame()
                    {
                        Name = boardGameDto.Name,
                        Rating = boardGameDto.Rating,
                        YearPublished = boardGameDto.YearPublished,
                        CategoryType = (CategoryType)boardGameDto.CategoryType,
                        Mechanics = boardGameDto.Mechanics
                    };

                    newCreator.Boardgames.Add(newBg);
                }

                validCreators.Add(newCreator);
                result.AppendLine(string.Format(SuccessfullyImportedCreator, newCreator.FirstName, newCreator.LastName, newCreator.Boardgames.Count));
            }

            context.Creators.AddRange(validCreators);
            context.SaveChanges();

            return result.ToString();
        }

        public static string ImportSellers(BoardgamesContext context, string jsonString)
        {
            StringBuilder result = new StringBuilder();

            ImportSellerDto[]? importSellersDtos = JsonConvert.DeserializeObject<ImportSellerDto[]>(jsonString);

            ICollection<Seller> validSellers = new HashSet<Seller>();
            int[] validBoardgamesIds = context.Boardgames.Select(bg => bg.Id).ToArray();

            foreach (var sellerDto in importSellersDtos)
            {
                if (!IsValid(sellerDto))
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                Seller newSeller = new Seller()
                {
                    Name = sellerDto.Name,
                    Address = sellerDto.Address,
                    Country = sellerDto.Country,
                    Website = sellerDto.Website
                };

                foreach (var bgId in sellerDto.Boardgames.Distinct())
                {
                    if (!validBoardgamesIds.Contains(bgId))
                    {
                        result.AppendLine(ErrorMessage);
                        continue;
                    }

                    newSeller.BoardgamesSellers.Add(new BoardgameSeller()
                    {
                        Seller = newSeller,
                        BoardgameId = bgId,
                    });
                }

                validSellers.Add(newSeller);

                result.AppendLine(string.Format(SuccessfullyImportedSeller, newSeller.Name, newSeller.BoardgamesSellers.Count));
            }

            context.Sellers.AddRange(validSellers);
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
