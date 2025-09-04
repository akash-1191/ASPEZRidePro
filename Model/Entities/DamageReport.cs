using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EZRide_Project.Model.Entities
{
    public class DamageReport
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DamageId { get; set; }
        public int BookingId { get; set; } // FK for Booking

        [Column(TypeName ="varchar(255)")]
        public string? Description { get; set; }

        [Column(TypeName ="decimal(10,2)")]
        public decimal? RepairCost { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string? Image { set; get; }

        [Column(TypeName ="varchar(80)")]
        public DamageStatus? Status { get; set; }=DamageStatus.Reported;// Enum can be used here
        public DateTime CreatedAt { get; set; }

        // Navigation property for Booking
        public Booking Booking { get; set; }


        public enum DamageStatus
        {
            Reported,
            Resolved
        }
    }
}
