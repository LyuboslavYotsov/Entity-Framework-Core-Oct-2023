namespace Footballers.DataProcessor.ExportDto
{
    public class ExportTeamWithFootballersDto
    {
        public string Name { get; set; } = null!;

        public ExportFootballersJsonDto[] Footballers { get; set; } = null!;
    }
}
