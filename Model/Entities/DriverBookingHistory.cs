using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EZRide_Project.Model.Entities
{
    public class DriverBookingHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DriverBookingId { get; set; }

        public int BookingId { get; set; } // FK → Bookings
        public int DriverId { get; set; } // FK → Drivers
        public int VehicleId { get; set; } // FK → Vehicles

        [Column(TypeName = "datetime2")]
        public DateTime AssignTime { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? StartTime { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? EndTime { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public AssignmentStatus Status { get; set; } // Enum: Assigned, InProgress, Completed, Cancelled

        public DateTime CreatedAt { get; set; }

        // Navigation properties
        [ForeignKey("BookingId")]
        public Booking Booking { get; set; } // Navigation property to Booking

        public Driver Driver { get; set; }
        public Vehicle Vehicle { get; set; }

        public enum AssignmentStatus
        {
            Assigned,
            InProgress,
            Completed,
            Cancelled
        }
    }
}
