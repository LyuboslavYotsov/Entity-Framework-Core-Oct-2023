using Artillery.Data.Models.Enums;
using Artillery.Data.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Artillery.DataProcessor.ImportDto
{
    public class ImportGunDto
    {
        [Required]
        public int ManufacturerId { get; set; }

        [Required]
        [Range(100, 1350000)]
        public int GunWeight { get; set; }

        [Required]
        [Range(2.00, 35.00)]
        public double BarrelLength { get; set; }

        public int? NumberBuild { get; set; }

        [Required]
        [Range(1, 100000)]
        public int Range { get; set; }

        [Required]
        public string GunType { get; set; } = null!;

        [Required]
        public int ShellId { get; set; }

        [JsonProperty("Countries")]
        public ImportCountryId[] Countries { get; set; } = null!;
    }
}
