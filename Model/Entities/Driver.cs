using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static EZRide_Project.Model.Entities.Vehicle;

namespace EZRide_Project.Model.Entities
{
    public class Driver
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DriverId { get; set; }

        public int UserId { get; set; } // FK → Users

        [Required]
        public int ExperienceYears { get; set; }

        [Required]
        [Column(TypeName = "varchar(20)")]
        public AvailabiliStatus AvailabilityStatus { get; set; } // Enum: Available, Busy, Inactive


        [Required]
        public VehicleType VehicleTypes { get; set; }

        [Required]
        [Column(TypeName = "varchar(20)")]
        public DriverStatus Status { get; set; } // Enum: Active, Disabled

        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public User User { get; set; }
        public ICollection<DriverDocuments> DriverDocuments { get; set; }
        public ICollection<DriverBookingHistory> DriverBookingHistories { get; set; }
        public ICollection<DriverReview> DriverReviews { get; set; }

        public enum AvailabiliStatus
        {
            Available,
            Busy,
            Inactive
        }

        public enum DriverStatus
        {
            Active,
            Disabled
        }
        public enum VehicleType
        {
            TwoWheeler = 0,
            FourWheeler = 1,
            Both = 2
        }
    }
}
