using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Trucks.Data.Models.Enums;

namespace Trucks.Data.Models
{
    public class Truck
    {
        [Key]
        public int Id { get; set; }

        [RegularExpression(@"^[A-Z]{2}[0-9]{4}[A-Z]{2}$")]
        [StringLength(8)]
        public string RegistrationNumber { get; set; } = null!;

        [Required]
        [StringLength(17)]
        public string VinNumber { get; set; } = null!;

        [Required]
        [Range(950, 1420)]
        public int TankCapacity { get; set; }

        [Range(5000,29000)]
        public int CargoCapacity { get; set; }

        [Range(0,3)]
        public CategoryType CategoryType { get; set; }

        [Required]
        [Range(0, 4)]
        public MakeType MakeType { get; set; }

        [Required]
        public int DespatcherId  { get; set; }

        [ForeignKey(nameof(DespatcherId))]
        public Despatcher Despatcher { get; set; } = null!;

        public virtual ICollection<ClientTruck> ClientsTrucks { get; set; } = new List<ClientTruck>();


    }
}
