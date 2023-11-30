namespace SoftJail.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
    using SoftJail.Extensions;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid Data";

        private const string SuccessfullyImportedDepartment = "Imported {0} with {1} cells";

        private const string SuccessfullyImportedPrisoner = "Imported {0} {1} years old";

        private const string SuccessfullyImportedOfficer = "Imported {0} ({1} prisoners)";

        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            StringBuilder res = new StringBuilder();

            ImportDepartmentDto[]? departmentsDtos = JsonConvert.DeserializeObject<ImportDepartmentDto[]>(jsonString);

            ICollection<Department> validDepartments = new HashSet<Department>();

            foreach (var departmentDto in departmentsDtos)
            {
                if (!IsValid(departmentDto) || departmentDto.Cells.Any(c => !IsValid(c)) || departmentDto.Cells.Length == 0)
                {
                    res.AppendLine(ErrorMessage);
                    continue;
                }

                Department newDepartment = new Department()
                {
                    Name = departmentDto.Name
                };

                foreach (var cellDto in departmentDto.Cells)
                {
                    Cell newCell = new Cell()
                    {
                        CellNumber = cellDto.CellNumber,
                        HasWindow = cellDto.HasWindow
                    };

                    newDepartment.Cells.Add(newCell);
                }

                validDepartments.Add(newDepartment);
                res.AppendLine(string.Format(SuccessfullyImportedDepartment, newDepartment.Name, newDepartment.Cells.Count));

            }

            context.Departments.AddRange(validDepartments);

            context.SaveChanges();

            return res.ToString();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            StringBuilder result = new StringBuilder();

            ImportPrisonerDto[]? prisonersDtos = JsonConvert.DeserializeObject<ImportPrisonerDto[]> (jsonString);

            ICollection<Prisoner> validPrisoners = new HashSet<Prisoner>();

            foreach (var prisonerDto in prisonersDtos)
            {
                bool incarcDateIsValid = DateTime.TryParseExact(prisonerDto.IncarcerationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime incDate);
                bool releaseDateIsValid = DateTime.TryParseExact(prisonerDto.IncarcerationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime relDate);

                if (!IsValid(prisonerDto) || prisonerDto.Mails.Any(m => !IsValid(m)) || !incarcDateIsValid)
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                Prisoner newPrisoner = new Prisoner()
                {
                    FullName = prisonerDto.FullName,
                    Nickname = prisonerDto.Nickname,
                    Age = prisonerDto.Age,
                    IncarcerationDate = incDate,
                    ReleaseDate = relDate,
                    Bail = prisonerDto.Bail,
                    CellId = prisonerDto.CellId
                };

                foreach (var mailDto in prisonerDto.Mails)
                {
                    Mail newMail = new Mail()
                    {
                        Description = mailDto.Description,
                        Sender = mailDto.Sender,
                        Address = mailDto.Address
                    };

                    newPrisoner.Mails.Add(newMail);
                }

                validPrisoners.Add(newPrisoner);
                result.AppendLine(string.Format(SuccessfullyImportedPrisoner, newPrisoner.FullName, newPrisoner.Age));
            }

            context.Prisoners.AddRange(validPrisoners);

            context.SaveChanges();

            return result.ToString();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            StringBuilder result = new StringBuilder();

            ImportOfficerDto[]? officersDtos = xmlString.DeserializeXml<ImportOfficerDto[]>("Officers");

            ICollection<Officer> validOfficers = new HashSet<Officer>();

            foreach (var officerDto in officersDtos)
            {
                bool positionIsValid = Enum.TryParse<Position>(officerDto.Position, out Position offPos);
                bool weaponIsValid = Enum.TryParse<Weapon>(officerDto.Weapon, out Weapon offWeap);

                if (!IsValid(officerDto) || !positionIsValid || !weaponIsValid)
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                Officer newOfficer = new Officer()
                {
                    FullName = officerDto.FullName,
                    Salary = officerDto.Salary,
                    Position = offPos,
                    Weapon = offWeap,
                    DepartmentId = officerDto.DepartmentId
                };

                foreach (var id in officerDto.Prisoners.Select(p => p.PrisonerId).ToArray().Distinct())
                {
                    OfficerPrisoner newOp = new OfficerPrisoner()
                    {
                        Officer = newOfficer,
                        PrisonerId = id
                    };

                    newOfficer.OfficerPrisoners.Add(newOp);
                }

                result.AppendLine(string.Format(SuccessfullyImportedOfficer, newOfficer.FullName, newOfficer.OfficerPrisoners.Count));
                validOfficers.Add(newOfficer);
            }

            context.Officers.AddRange(validOfficers);

            context.SaveChanges();

            return result.ToString();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}