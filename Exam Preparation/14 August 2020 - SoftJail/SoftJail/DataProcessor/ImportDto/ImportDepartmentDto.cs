using System.ComponentModel.DataAnnotations;

namespace SoftJail.DataProcessor.ImportDto
{
    public class ImportDepartmentDto
    {
        [Required]
        [StringLength(25, MinimumLength = 3)]
        public string Name { get; set; } = null!;

        public ImportCellDto[] Cells { get; set; } = null!;
    }
}
