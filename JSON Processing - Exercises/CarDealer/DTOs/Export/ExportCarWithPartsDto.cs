using Newtonsoft.Json;

namespace CarDealer.DTOs.Export
{
    public class ExportCarWithPartsDto
    {
        [JsonProperty("car")]
        public ExportCarDto2 Car { get; set; }

        [JsonProperty("parts")]
        public ExportPartDto[] Parts { get; set; }
    }
}
