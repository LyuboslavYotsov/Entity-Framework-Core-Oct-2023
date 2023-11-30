namespace Theatre.DataProcessor
{
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using Theatre.Data;
    using Theatre.Data.Models;
    using Theatre.Data.Models.Enums;
    using Theatre.DataProcessor.ImportDto;
    using Theatre.Extensions;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfulImportPlay
            = "Successfully imported {0} with genre {1} and a rating of {2}!";

        private const string SuccessfulImportActor
            = "Successfully imported actor {0} as a {1} character!";

        private const string SuccessfulImportTheatre
            = "Successfully imported theatre {0} with #{1} tickets!";



        public static string ImportPlays(TheatreContext context, string xmlString)
        {
            StringBuilder result = new StringBuilder();

            ImportPlayDto[]? playsDtos = xmlString.DeserializeXml<ImportPlayDto[]>("Plays");

            ICollection<Play> validPlays = new List<Play>();

            string[] validGenres = Enum.GetNames(typeof(Genre));

            foreach (var playDto in playsDtos)
            {
                TimeSpan time = TimeSpan.ParseExact(playDto.Duration, "c", CultureInfo.InvariantCulture);

                if (!IsValid(playDto) || time < TimeSpan.FromHours(1) || !validGenres.Contains(playDto.Genre))
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                Play newPlay = new Play()
                {
                    Title = playDto.Title,
                    Duration = time,
                    Rating = playDto.Rating,
                    Genre = Enum.Parse<Genre>(playDto.Genre),
                    Description = playDto.Description,
                    Screenwriter = playDto.Screenwriter
                };

                validPlays.Add(newPlay);
                result.AppendLine(string.Format(SuccessfulImportPlay, newPlay.Title, newPlay.Genre.ToString(), newPlay.Rating));
            }

            context.Plays.AddRange(validPlays);

            context.SaveChanges();

            return result.ToString();
        }

        public static string ImportCasts(TheatreContext context, string xmlString)
        {
            StringBuilder result = new StringBuilder();

            ImportCastDto[]? CastsDtos = xmlString.DeserializeXml<ImportCastDto[]>("Casts");

            ICollection<Cast> validCasts = new List<Cast>();

            foreach (var castDto in CastsDtos)
            {
                if (!IsValid(castDto))
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                Cast newCast = new Cast()
                {
                    FullName = castDto.FullName,
                    IsMainCharacter = castDto.IsMainCharacter,
                    PhoneNumber = castDto.PhoneNumber,
                    PlayId = castDto.PlayId
                };

                validCasts.Add(newCast);
                result.AppendLine(string.Format(SuccessfulImportActor, newCast.FullName, newCast.IsMainCharacter ? "main" : "lesser"));
            }

            context.Casts.AddRange(validCasts);

            context.SaveChanges();

            return result.ToString();
        }

        public static string ImportTtheatersTickets(TheatreContext context, string jsonString)
        {
            StringBuilder result = new StringBuilder();

            ImportTheatreDto[]? TheatresDtos = JsonConvert.DeserializeObject<ImportTheatreDto[]>(jsonString);

            ICollection<Theatre> validTheatres = new List<Theatre>();

            foreach (var theatreDto in TheatresDtos)
            {
                if (!IsValid(theatreDto))
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                Theatre newTheatre = new Theatre()
                {
                    Name = theatreDto.Name,
                    NumberOfHalls = theatreDto.NumberOfHalls,
                    Director = theatreDto.Director
                };

                foreach (var ticketDto in theatreDto.Tickets)
                {
                    if (!IsValid(ticketDto))
                    {
                        result.AppendLine(ErrorMessage);
                        continue;
                    }

                    Ticket newTicket = new Ticket()
                    {
                        Price = ticketDto.Price,
                        RowNumber = ticketDto.RowNumber,
                        PlayId = ticketDto.PlayId
                    };

                    newTheatre.Tickets.Add(newTicket);
                }

                validTheatres.Add(newTheatre);
                result.AppendLine(string.Format(SuccessfulImportTheatre, newTheatre.Name, newTheatre.Tickets.Count));
            }

            context.Theatres.AddRange(validTheatres);

            context.SaveChanges();

            return result.ToString();
        }


        private static bool IsValid(object obj)
        {
            var validator = new ValidationContext(obj);
            var validationRes = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, validator, validationRes, true);
            return result;
        }
    }
}
