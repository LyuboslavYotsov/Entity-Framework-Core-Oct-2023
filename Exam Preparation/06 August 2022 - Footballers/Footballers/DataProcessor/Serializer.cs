namespace Footballers.DataProcessor
{
    using Data;
    using Footballers.Data.Models.Enums;
    using Footballers.DataProcessor.ExportDto;
    using Footballers.Extensions;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using System.Globalization;

    public class Serializer
    {
        public static string ExportCoachesWithTheirFootballers(FootballersContext context)
        {
            var coachesWithFootballers = context.Coaches
                .Where(c => c.Footballers.Count > 0)
                .Select(c => new ExportCoachDto()
                {
                    CoachName = c.Name,
                    FootballersCount = c.Footballers.Count,
                    Footballers = c.Footballers
                                   .Select(f => new ExportFootballerXmlDto()
                                   {
                                       Name = f.Name,
                                       Position = ((PositionType)f.PositionType).ToString()
                                   })
                                   .OrderBy(f => f.Name)
                                   .ToArray()
                })
                .OrderByDescending(ec => ec.FootballersCount)
                .ThenBy(ec => ec.CoachName)
                .ToArray();

            string result = coachesWithFootballers.SerializeXml("Coaches");

            return result;
        }

        public static string ExportTeamsWithMostFootballers(FootballersContext context, DateTime date)
        {
            var teamsWithFootballers = context.Teams
                .AsNoTracking()
                .Where(t => t.TeamsFootballers.Any(tf => tf.Footballer.ContractStartDate >= date))
                .Select(t => new ExportTeamWithFootballersDto()
                {
                    Name = t.Name,
                    Footballers = t.TeamsFootballers
                                    .Where(tf => tf.Footballer.ContractStartDate >= date)
                                    .ToArray()
                                    .OrderByDescending(tf => tf.Footballer.ContractEndDate)
                                    .ThenBy(tf => tf.Footballer.Name)
                                    .Select(tf => new ExportFootballersJsonDto()
                                    {
                                        FootballerName = tf.Footballer.Name,
                                        ContractStartDate = tf.Footballer.ContractStartDate.ToString("d", CultureInfo.InvariantCulture),
                                        ContractEndDate = tf.Footballer.ContractEndDate.ToString("d", CultureInfo.InvariantCulture),
                                        BestSkillType = tf.Footballer.BestSkillType.ToString(),
                                        PositionType = tf.Footballer.PositionType.ToString()
                                    })
                                    .ToArray()
                })
                .OrderByDescending(t => t.Footballers.Length)
                .ThenBy(t => t.Name)
                .Take(5)
                .ToArray();

            string result = JsonConvert.SerializeObject(teamsWithFootballers, Formatting.Indented);

            return result;
        }
    }
}
