using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EZRide_Project.Model.Entities
{
    public class SecurityDeposit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DepositId { get; set; }
        public int BookingId { get; set; } // FK

        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Column(TypeName ="varchar(50)")]
        public DepositStatus Status { get; set; } // Enum can be used here
        public DateTime CreatedAt { get; set; }
        public DateTime? RefundedAt { get; set; }

        // Navigation property for Booking
        public Booking Booking { get; set; }


        public enum DepositStatus
        {
            Pending,
            Refunded,
            Forfeited
        }
    }
}
