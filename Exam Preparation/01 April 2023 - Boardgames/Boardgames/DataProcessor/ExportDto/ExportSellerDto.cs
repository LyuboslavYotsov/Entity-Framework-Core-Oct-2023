namespace Boardgames.DataProcessor.ExportDto
{
    public class ExportSellerDto
    {
        public string Name { get; set; } = null!;

        public string Website { get; set; } = null!;

        public ExportBoardgameDto[] Boardgames { get; set; } = null!;
    }
}
