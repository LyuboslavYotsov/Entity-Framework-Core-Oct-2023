// ReSharper disable InconsistentNaming

namespace TeisterMask.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    using Data;
    using System.Text;
    using TeisterMask.DataProcessor.ImportDto;
    using TeisterMask.Extensions;
    using TeisterMask.Data.Models;
    using System.Globalization;
    using Castle.Core.Internal;
    using TeisterMask.Data.Models.Enums;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            StringBuilder result = new StringBuilder();

            ImportProjectDto[]? projectsDtos = xmlString.DeserializeXml<ImportProjectDto[]>("Projects");

            ICollection<Project> validProjects = new List<Project>();

            foreach (var projectDto in projectsDtos)
            {
                DateTime pod;

                if (!IsValid(projectDto) || !DateTime.TryParseExact(projectDto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture ,DateTimeStyles.None , out pod))
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                Project newProject = new Project()
                {
                    Name = projectDto.Name,
                    OpenDate = pod,
                    DueDate = projectDto.DueDate.IsNullOrEmpty() ? null : DateTime.ParseExact(projectDto.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                };

                foreach (var taskDto in projectDto.Tasks)
                {
                    DateTime taskOd;
                    DateTime taskDd;

                    if (!IsValid(taskDto) || taskDto.DueDate.IsNullOrEmpty() || taskDto.OpenDate.IsNullOrEmpty() 
                        || !DateTime.TryParseExact(taskDto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out taskOd) 
                        || !DateTime.TryParseExact(taskDto.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out taskDd) 
                        || taskOd < pod || taskDd > newProject.DueDate)
                    {
                        result.AppendLine(ErrorMessage);
                        continue;
                    }

                    Task newTask = new Task()
                    {
                        Name = taskDto.Name,
                        OpenDate = taskOd,
                        DueDate = taskDd,
                        ExecutionType = (ExecutionType)taskDto.ExecutionType,
                        LabelType = (LabelType)taskDto.LabelType
                    };

                    newProject.Tasks.Add(newTask);
                }

                validProjects.Add(newProject);

                result.AppendLine(string.Format(SuccessfullyImportedProject, newProject.Name, newProject.Tasks.Count));

            }

            context.Projects.AddRange(validProjects);

            context.SaveChanges();

            return result.ToString();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            StringBuilder result = new StringBuilder();

            ImportEmployeeDto[]? employeesDtos = JsonConvert.DeserializeObject<ImportEmployeeDto[]>(jsonString);

            int[] validTaskIds = context.Tasks.Select(t => t.Id).ToArray();

            ICollection<Employee> validEmployees = new List<Employee>();

            foreach (var employeeDto in employeesDtos)
            {
                if (!IsValid(employeeDto))
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                Employee newEmployee = new Employee()
                {
                    Username = employeeDto.Username,
                    Email = employeeDto.Email,
                    Phone = employeeDto.Phone
                };

                foreach (var taskId in employeeDto.Tasks.Distinct())
                {
                    if (!validTaskIds.Contains(taskId))
                    {
                        result.AppendLine(ErrorMessage);
                        continue;
                    }

                    newEmployee.EmployeesTasks.Add(new EmployeeTask()
                    {
                        Employee = newEmployee,
                        TaskId = taskId
                    });
                }

                validEmployees.Add(newEmployee);
                result.AppendLine(string.Format(SuccessfullyImportedEmployee, newEmployee.Username, newEmployee.EmployeesTasks.Count));
            }

            context.Employees.AddRange(validEmployees);

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