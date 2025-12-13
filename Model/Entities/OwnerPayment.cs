using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EZRide_Project.Model.Entities
{
    public class OwnerPayment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OwnerPaymentId { get; set; }
        public int UserId { get; set; } // FK for User (Owner)
        public int VehicleId { get; set; } 

        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Required]
        [Column(TypeName ="varchar(50)")]
        public paymentType PaymentType { get; set; } // Enum can be used here

        [Required]
        [Column(TypeName = "varchar(50)")]
        public PaymentStatus Status { get; set; } // Enum can be used here

        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Vehicle Vehicle { get; set; }



        public enum paymentType
        {
            Cash,
            Online
        }
        public enum PaymentStatus
        {
            Pending,
            Paid,
            ReRent
        }
    }
}
