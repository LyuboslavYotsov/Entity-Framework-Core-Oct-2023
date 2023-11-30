namespace TeisterMask.DataProcessor
{
    using Data;
    using Microsoft.VisualBasic;
    using Newtonsoft.Json;
    using System.Globalization;
    using TeisterMask.Data.Models.Enums;
    using TeisterMask.DataProcessor.ExportDto;
    using TeisterMask.Extensions;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            var projectsWithTasks = context.Projects
                .Where(p => p.Tasks.Count > 0)
                .Select(p => new ExportProjectDto()
                {
                    TasksCount = p.Tasks.Count,
                    ProjectName = p.Name,
                    HasEndDate = p.DueDate != null ? "Yes" : "No",
                    Tasks = p.Tasks.Select(t => new ExportTaskDto()
                    {
                        Name = t.Name,
                        Label = t.LabelType.ToString(),
                    })
                    .OrderBy(t => t.Name)
                    .ToArray()
                })
                .OrderByDescending(ep => ep.TasksCount)
                .ThenBy(ep => ep.ProjectName)
                .ToArray();

            return projectsWithTasks.SerializeXml("Projects");
        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            var busiestEmployees = context.Employees
                .Where(e => e.EmployeesTasks.Any(t => t.Task.OpenDate >= date))
                .OrderByDescending(ee => ee.EmployeesTasks.Count)
                    .ThenBy(ee => ee.Username)
                .ToArray()
                .Select(e => new
                {
                    Username = e.Username,
                    Tasks = e.EmployeesTasks
                             .Where(et => et.Task.OpenDate >= date)
                             .ToArray()
                             .OrderByDescending(et => et.Task.DueDate)
                                .ThenBy(et => et.Task.Name)
                             .Select(et => new
                             {
                                 TaskName = et.Task.Name,
                                 OpenDate = et.Task.OpenDate.ToString("d", CultureInfo.InvariantCulture),
                                 DueDate = et.Task.DueDate.ToString("d", CultureInfo.InvariantCulture),
                                 LabelType = et.Task.LabelType.ToString(),
                                 ExecutionType = et.Task.ExecutionType.ToString(),
                             })
                             .ToArray()
                })
                .Take(10)
                .ToArray();

            return JsonConvert.SerializeObject(busiestEmployees, Formatting.Indented);
        }
    }
}