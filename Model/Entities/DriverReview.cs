using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EZRide_Project.Model.Entities
{
    public class DriverReview
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReviewId { get; set; }

        public int DriverId { get; set; } // FK → Drivers
        public int UserId { get; set; }   // FK → Users

        [Column(TypeName = "decimal(3,2)")]
        public decimal Rating { get; set; } // 1.0 to 5.0

        [Column(TypeName = "text")]
        public string? Feedback { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Driver Driver { get; set; }
        public User User { get; set; }
    }
}
