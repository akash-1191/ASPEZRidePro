using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EZRide_Project.Model.Entities
{
    public class BookingOTP
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Booking")]
        public int BookingId { get; set; }

        [Column(TypeName = "varchar(6)")]
        public string OTPCode { get; set; }

        public DateTime ExpiryTime { get; set; }

        public bool IsUsed { get; set; } = false;
        [Column(TypeName = "varchar(100)")]
        public string EmailSentTo { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public Booking Booking { get; set; }
    }
}
