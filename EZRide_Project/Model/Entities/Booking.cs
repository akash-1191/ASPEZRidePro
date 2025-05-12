using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EZRide_Project.Model.Entities
{
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookingId { get; set; }
        public int UserId { get; set; } //FK
        public int VehicleId { get; set; } // FK

        [Column(TypeName = "datetime2")]
        public DateTime StartTime { get; set; }


        [Column(TypeName = "datetime2")]
        public DateTime EndTime { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalDistance { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [Column(TypeName ="varchar(50)")]
        public BookingStatus Status { get; set; }//use ENUM
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Vehicle Vehicle { get; set; }
        public Payment Payment { get; set; }
        public SecurityDeposit SecurityDeposit { get; set; }
        public FuelLog FuelLog { get; set; }
        public DamageReport DamageReport { get; set; }



        public enum BookingStatus
        {
            Pending,
            Confirmed,
            InProgress,
            Completed,
            Cancelled
        }
     
    }
}
