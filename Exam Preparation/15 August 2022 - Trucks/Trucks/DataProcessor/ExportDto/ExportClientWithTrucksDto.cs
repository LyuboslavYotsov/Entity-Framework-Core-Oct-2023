namespace Trucks.DataProcessor.ExportDto
{
    public class ExportClientWithTrucksDto
    {
        public string Name { get; set; } = null!;

        public ExportTruckDto[] Trucks { get; set; } = null!;
    }
}
