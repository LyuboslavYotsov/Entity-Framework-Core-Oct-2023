namespace Theatre.DataProcessor
{
    using Newtonsoft.Json;
    using System;
    using System.Globalization;
    using Theatre.Data;
    using Theatre.Data.Models.Enums;
    using Theatre.DataProcessor.ExportDto;
    using Theatre.Extensions;

    public class Serializer
    {
        public static string ExportTheatres(TheatreContext context, int numbersOfHalls)
        {
            var topTheatres = context.Theatres
                .Where(t => t.NumberOfHalls >= numbersOfHalls && t.Tickets.Count >= 20)
                .Select(t => new
                {
                    Name = t.Name,
                    Halls = t.NumberOfHalls,
                    TotalIncome = t.Tickets.Where(ti => ti.RowNumber > 0 && ti.RowNumber <= 5).Sum(ti => ti.Price),
                    Tickets = t.Tickets.Where(ti => ti.RowNumber > 0 && ti.RowNumber <= 5)
                                       .Select(ti => new
                                       {
                                           Price = ti.Price,
                                           RowNumber = ti.RowNumber
                                       })
                                       .OrderByDescending(ti => ti.Price)
                                       .ToArray()
                })
                .OrderByDescending(et => et.Halls)
                    .ThenBy(et => et.Name)
                .ToArray();

            return JsonConvert.SerializeObject(topTheatres, Formatting.Indented);
        }

        public static string ExportPlays(TheatreContext context, double raiting)
        {
            var playWithMainCharacters = context.Plays
                .ToArray()
                .Where(p => p.Rating <= raiting)
                .Select(p => new ExportPlayDto()
                {
                    Title = p.Title,
                    Duration = p.Duration.ToString("c", CultureInfo.InvariantCulture),
                    Rating = p.Rating == 0 ? "Premier" : p.Rating.ToString(),
                    Genre = ((Genre)p.Genre).ToString(),
                    Actors = p.Casts
                                .Where(c => c.IsMainCharacter)
                                .Select(c => new ExportActorDto()
                                {
                                   FullName = c.FullName,
                                   MainCharacter = "Plays main character in '" + p.Title + "'."
                                })
                                .OrderByDescending(a => a.FullName)
                                .ToArray()
                })
                .OrderBy(ep => ep.Title)
                    .ThenByDescending(ep => ep.Genre)
                .ToArray();

            return playWithMainCharacters.SerializeXml("Plays");
        }
    }
}
