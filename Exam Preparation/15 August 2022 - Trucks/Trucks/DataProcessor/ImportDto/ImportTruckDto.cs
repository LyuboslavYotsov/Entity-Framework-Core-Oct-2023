using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Trucks.Data.Models;
using Trucks.Data.Models.Enums;

namespace Trucks.DataProcessor.ImportDto
{
    [XmlType(nameof(Truck))]
    public class ImportTruckDto
    {
        [Required]
        [RegularExpression(@"^[A-Z]{2}[0-9]{4}[A-Z]{2}$")]
        [StringLength(8)]
        public string RegistrationNumber { get; set; } = null!;

        [Required]
        [StringLength(17)]
        public string VinNumber { get; set; } = null!;

        [Required]
        [Range(950, 1420)]
        public int TankCapacity { get; set; }

        [Range(5000, 29000)]
        public int CargoCapacity { get; set; }

        [Range(0, 3)]
        public int CategoryType { get; set; }

        [Required]
        [Range(0, 4)]
        public int MakeType { get; set; }
    }
}
