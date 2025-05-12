using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EZRide_Project.Model.Entities
{
    public class PricingRule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PricingId { get; set; }
        public int VehicleId { get; set; } // Foreign key for Vehicle

        [Column(TypeName = "decimal(10,2)")]
        public decimal PricePerKm { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PricePerHour { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PricePerDay { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation property for Vehicle
        public Vehicle Vehicle { get; set; }
    }
}
