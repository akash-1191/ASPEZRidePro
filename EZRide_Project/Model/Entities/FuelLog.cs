using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EZRide_Project.Model.Entities
{
    public class FuelLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FuelLogId { get; set; }
        public int BookingId { get; set; } 

        [Column(TypeName = "decimal(5,2)")]
        public decimal? FuelGiven { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? FuelReturned { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? FuelCharge { get; set; }

        [Column(TypeName = "varchar(50)")]
        public FuelLogStatus? Status { get; set; } = FuelLogStatus.Active; // Enum can be used here
        public DateTime CreatedAt { get; set; }

        // Navigation property for Booking
        public Booking Booking { get; set; }

        public enum FuelLogStatus
        {
            Active,
            Cancelled
        }
      
    }
}
