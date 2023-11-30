namespace Footballers.DataProcessor.ExportDto
{
    public class ExportFootballersJsonDto
    {
        public string FootballerName { get; set; } = null!;
        public string ContractStartDate { get; set; } = null!;
        public string ContractEndDate { get; set; } = null!;
        public string BestSkillType { get; set; } = null!;
        public string PositionType { get; set; } = null!;
    }
}
