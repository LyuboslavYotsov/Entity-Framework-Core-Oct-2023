namespace Boardgames.DataProcessor.ExportDto
{
    public class ExportBoardgameDto
    {
        public string Name { get; set; } = null!;

        public double Rating { get; set; }

        public string Mechanics { get; set; } = null!;

        public string Category { get; set; } = null!;
    }
}
