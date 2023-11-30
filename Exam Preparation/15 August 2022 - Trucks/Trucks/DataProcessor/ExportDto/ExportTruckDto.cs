using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using Trucks.Data.Models.Enums;

namespace Trucks.DataProcessor.ExportDto
{
    public class ExportTruckDto
    {
        public string TruckRegistrationNumber { get; set; } = null!;

        public string VinNumber { get; set; } = null!;

        public int TankCapacity { get; set; }

        public int CargoCapacity { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public CategoryType CategoryType { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public MakeType MakeType { get; set; }
    }
}
