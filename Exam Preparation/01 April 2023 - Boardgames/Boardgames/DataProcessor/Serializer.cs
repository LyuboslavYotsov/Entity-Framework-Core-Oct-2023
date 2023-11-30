namespace Boardgames.DataProcessor
{
    using Boardgames.Data;
    using Boardgames.DataProcessor.ExportDto;
    using Boardgames.Extensions;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportCreatorsWithTheirBoardgames(BoardgamesContext context)
        {
            ExportCreatorDto[] creatorsWithGames = context.Creators
                .AsNoTracking()
                .Where(c => c.Boardgames.Count > 0)
                .Select(c => new ExportCreatorDto()
                {
                    CreatorName = c.FirstName + " " + c.LastName,
                    BoardgamesCount = c.Boardgames.Count,
                    Boardgames = c.Boardgames
                        .Select(bg => new ExportBoardgameXmlDto()
                        {
                            BoardgameName = bg.Name,
                            BoardgameYearPublished = bg.YearPublished
                        })
                        .OrderBy(bg => bg.BoardgameName)
                        .ToArray()
                })
                .OrderByDescending(ec => ec.BoardgamesCount)
                .ThenBy(ec => ec.CreatorName)
                .ToArray();

            string result = creatorsWithGames.SerializeXml("Creators");

            return result;
        }

        public static string ExportSellersWithMostBoardgames(BoardgamesContext context, int year, double rating)
        {
            var sellersWithBoardgames = context.Sellers
                .Where(s => s.BoardgamesSellers.Any(bs => bs.Boardgame.YearPublished >= year && bs.Boardgame.Rating <= rating))
                .Select(s => new ExportSellerDto()
                {
                    Name = s.Name,
                    Website = s.Website,
                    Boardgames = s.BoardgamesSellers
                                  .Where(bs => bs.Boardgame.YearPublished >= year && bs.Boardgame.Rating <= rating)
                                  .Select(bs => new ExportBoardgameDto()
                                  {
                                      Name = bs.Boardgame.Name,
                                      Rating = bs.Boardgame.Rating,
                                      Mechanics = bs.Boardgame.Mechanics,
                                      Category = bs.Boardgame.CategoryType.ToString()

                                  })
                                  .OrderByDescending(bg => bg.Rating)
                                  .ThenBy(bg => bg.Name)
                                  .ToArray()
                })
                .OrderByDescending(es => es.Boardgames.Count())
                .ThenBy(es => es.Name)
                .Take(5)
                .ToArray();

            string result = JsonConvert.SerializeObject(sellersWithBoardgames, Formatting.Indented);

            return result;
        }
    }
}