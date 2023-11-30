namespace SoftJail.DataProcessor
{
    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.DataProcessor.ExportDto;
    using SoftJail.Extensions;
    using System.Globalization;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            var prisonersByCells = context.Prisoners
                .Where(p => ids.Contains(p.Id))
                .Select(p => new
                {
                    Id = p.Id,
                    Name = p.FullName,
                    CellNumber = p.Cell.CellNumber,
                    Officers = p.PrisonerOfficers.Select(po => new
                    {
                        OfficerName = po.Officer.FullName,
                        Department = po.Officer.Department.Name,
                    })
                    .OrderBy(eo => eo.OfficerName)
                    .ToArray(),
                    TotalOfficerSalary = p.PrisonerOfficers.Sum(op => op.Officer.Salary)
                })
                .OrderBy(ep => ep.Name)
                .ThenBy(ep => ep.Id)
                .ToArray();

            return JsonConvert.SerializeObject(prisonersByCells, Formatting.Indented);
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            string[] targetNames = prisonersNames.Split(',');

            ExportPrisonerDto[] prisonerWithMessages = context.Prisoners
                .Where(p => targetNames.Contains(p.FullName))
                .Select(p => new ExportPrisonerDto()
                {
                    Id = p.Id,
                    Name = p.FullName,
                    IncarcerationDate = p.IncarcerationDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    EncryptedMessages = p.Mails.Select(m => new ExportMessageDto
                    {
                        Description = m.Description
                    })
                    .ToArray()
                })
                .OrderBy(ep => ep.Name)
                .ThenBy(ep => ep.Id)
                .ToArray();

            foreach (var prisoner in prisonerWithMessages)
            {
                foreach (var message in prisoner.EncryptedMessages)
                {
                    char[] msgarray = message.Description.ToCharArray();

                    Array.Reverse(msgarray);

                    string reversed = new string(msgarray);

                    message.Description = reversed;
                }
            }

            return prisonerWithMessages.SerializeXml("Prisoners");

        }
    }
}