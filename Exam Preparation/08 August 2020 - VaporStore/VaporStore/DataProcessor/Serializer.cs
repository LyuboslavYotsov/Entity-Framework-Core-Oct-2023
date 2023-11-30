namespace VaporStore.DataProcessor
{
    using Data;
    using Newtonsoft.Json;
    using System.Globalization;
    using VaporStore.Data.Models;
    using VaporStore.Data.Models.Enums;
    using VaporStore.DataProcessor.ExportDto;
    using VaporStore.Extensions;

    public static class Serializer
    {
        public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
        {
            var gamesByGenres = context.Genres
                .Where(g => genreNames.Contains(g.Name))
                .ToArray()
                .Select(g => new
                {
                    Id = g.Id,
                    Genre = g.Name,
                    Games = g.Games
                             .Where(game => game.Purchases.Any())
                             .ToArray()
                             .Select(game => new
                             {
                                 Id = game.Id,
                                 Title = game.Name,
                                 Developer = game.Developer.Name,
                                 Tags = string.Join(", ", game.GameTags.Select(gt => gt.Tag.Name).ToArray()),
                                 Players = game.Purchases.Count
                             })
                             .OrderByDescending(egame => egame.Players)
                             .ThenBy(egame => egame.Id)
                             .ToArray()
                })
                .ToArray();

            var genresWithPlayersCount = gamesByGenres
                .Select(eg => new
                {
                    Id = eg.Id,
                    Genre = eg.Genre,
                    Games = eg.Games,
                    TotalPlayers = eg.Games.Sum(g => g.Players)
                })
                .OrderByDescending(eg => eg.TotalPlayers)
                .ThenBy (eg => eg.Id)
                .ToArray();

            return JsonConvert.SerializeObject(genresWithPlayersCount, Formatting.Indented);
        }

        public static string ExportUserPurchasesByType(VaporStoreDbContext context, string purchaseType)
        {

            var usersByPurchaseType = context.Users
                .Where(u => u.Cards.Any(c => c.Purchases.Count > 0))
                .ToArray()
                .Select(u => new ExportUserDto()
                {
                    UserName = u.Username,
                    Purchases = u.Cards.SelectMany(c => c.Purchases)
                                 .Where(p => p.Type.ToString() == purchaseType)
                                 .OrderBy(p => p.Date)
                                 .Select(p => new ExportPurchaseDto()
                                 {
                                     CardNumber = p.Card.Number,
                                     CardCvc = p.Card.Cvc,
                                     Date = p.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                                     Game = new ExportGameDto()
                                     {
                                         GameTitle = p.Game.Name,
                                         Genre = p.Game.Genre.Name,
                                         Price = p.Game.Price
                                     }
                                 })
                                 .ToArray()
                })
                .ToArray();
                

            var usersWithTotalSpentMoney = usersByPurchaseType
                .Where(u => u.Purchases.Any())
                .Select(u => new ExportUsersWithTotalSpentDto()
                {
                    UserName = u.UserName,
                    Purchases = u.Purchases,
                    TotalSpent = u.Purchases.Sum(p => p.Game.Price)
                })
                .OrderByDescending(u => u.TotalSpent)
                .ThenBy(u => u.UserName)
                .ToArray();


            return usersWithTotalSpentMoney.SerializeXml("Users");
        }
    }
}