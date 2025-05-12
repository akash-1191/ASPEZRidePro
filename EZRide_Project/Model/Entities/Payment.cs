using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EZRide_Project.Model.Entities
{
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookingId { get; set; } // Foreign key for Booking

        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }
        public Paymentmethod PaymentMethod { get; set; } // Enum can be used here

        [Required]
        [Column(TypeName ="varchar(100)")]
        public string TransactionId { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public PaymentStatus Status { get; set; } // Enum can be used here
        public DateTime CreatedAt { get; set; }

        // Navigation property for Booking
        public Booking Booking { get; set; }


        public enum Paymentmethod
        {
            Cash,
            Online
        }

        public enum PaymentStatus
        {
            Pending,
            Success,
            Failed
        }
    }
}
