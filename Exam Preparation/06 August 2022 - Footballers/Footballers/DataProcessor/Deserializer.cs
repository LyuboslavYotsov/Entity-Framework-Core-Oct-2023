namespace Footballers.DataProcessor
{
    using Footballers.Data;
    using Footballers.Data.Models;
    using Footballers.Data.Models.Enums;
    using Footballers.DataProcessor.ImportDto;
    using Footballers.Extensions;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCoach
            = "Successfully imported coach - {0} with {1} footballers.";

        private const string SuccessfullyImportedTeam
            = "Successfully imported team - {0} with {1} footballers.";

        public static string ImportCoaches(FootballersContext context, string xmlString)
        {
            StringBuilder result = new StringBuilder();

            var coachesDtos = xmlString.DeserializeXml<ImportCoachDto[]>("Coaches");
            ICollection<Coach> validCoaches = new HashSet<Coach>();

            foreach (var coachDto in coachesDtos)
            {
                if (!IsValid(coachDto))
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                Coach newCoach = new Coach()
                {
                    Name = coachDto.Name,
                    Nationality = coachDto.Nationality
                };

                foreach (var footballerDto in coachDto.Footballers)
                {
                    if (!IsValid(footballerDto))
                    {
                        result.AppendLine(ErrorMessage);
                        continue;
                    }

                    var startDate = DateTime.ParseExact(footballerDto.ContractStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    var endDate = DateTime.ParseExact(footballerDto.ContractEndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    if (startDate > endDate || startDate == null || endDate == null)
                    {
                        result.AppendLine(ErrorMessage);
                        continue;
                    }

                    Footballer newFootballer = new Footballer()
                    {
                        Name= footballerDto.Name,
                        ContractStartDate = startDate,
                        ContractEndDate = endDate,
                        BestSkillType = (BestSkillType)footballerDto.BestSkillType,
                        PositionType = (PositionType)footballerDto.PositionType
                    };

                    newCoach.Footballers.Add(newFootballer);
                }

                validCoaches.Add(newCoach);
                result.AppendLine(string.Format(SuccessfullyImportedCoach, newCoach.Name, newCoach.Footballers.Count));
            }

            context.Coaches.AddRange(validCoaches);

            context.SaveChanges();

            return result.ToString();
        }

        public static string ImportTeams(FootballersContext context, string jsonString)
        {
            StringBuilder result = new StringBuilder();

            int[] validFootballersIds = context.Footballers.Select(x => x.Id).ToArray();
            ICollection<Team> validTeams = new List<Team>();

            ImportTeamDto[]? teamsDtos = JsonConvert.DeserializeObject<ImportTeamDto[]>(jsonString);

            foreach (var teamDto in teamsDtos)
            {
                if (!IsValid(teamDto))
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                Team newTeam = new Team()
                {
                    Name = teamDto.Name,
                    Nationality = teamDto.Nationality,
                    Trophies = teamDto.Trophies
                };

                foreach (var footballerId in teamDto.Footballers.Distinct())
                {
                    if (!validFootballersIds.Contains(footballerId))
                    {
                        result.AppendLine(ErrorMessage);
                        continue;
                    }

                    newTeam.TeamsFootballers.Add(new TeamFootballer()
                    {
                        FootballerId = footballerId
                    });
                }

                validTeams.Add(newTeam);
                result.AppendLine(string.Format(SuccessfullyImportedTeam, newTeam.Name, newTeam.TeamsFootballers.Count));
            }

            context.Teams.AddRange(validTeams);

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
