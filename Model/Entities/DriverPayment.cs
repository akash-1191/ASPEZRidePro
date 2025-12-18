using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EZRide_Project.Model.Entities
{
    public class DriverPayment
    {

        [Key]
        public int DriverPaymentId { get; set; }

        [Required]
        public int DriverId { get; set; }

        [Required]
        public int BookingId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(20)]
        public string PaymentType { get; set; } // cash / online

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } // pending / paid

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? PaidAt { get; set; }

        //  Navigation Properties (optional but safe)
        public Driver Driver { get; set; }
        public Booking Booking { get; set; }

    }
}
