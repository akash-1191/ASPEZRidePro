using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EZRide_Project.Model.Entities
{
    public class OwnerVehicleAvailability
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AvailabilityId { get; set; }

        public int VehicleId { get; set; } // FK → Vehicles
        public int OwnerId { get; set; }   // FK → Users

        public int AvailableDays { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime EffectiveFrom { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime EffectiveTo { get; set; }

        [Required]
        [Column(TypeName = "varchar(20)")]
        public AvailabilityStatus Status { get; set; } // Enum: Active, Expired

        [Column(TypeName = "decimal(10,2)")]
        public decimal vehicleAmountPerDay { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Vehicle Vehicle { get; set; }
        public User Owner { get; set; }

        public enum AvailabilityStatus
        {
            Active,
            Expired
        }
    }
}
