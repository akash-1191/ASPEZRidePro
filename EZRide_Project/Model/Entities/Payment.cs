using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EZRide_Project.Model.Entities
{
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }  // Primary key

        public int BookingId { get; set; } // Foreign key for Booking

        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        public string TransactionId { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string? OrderId { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string PaymentMethod { get; set; }


        [Required]
        [Column(TypeName = "varchar(50)")]
        public string Status { get; set; }
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
