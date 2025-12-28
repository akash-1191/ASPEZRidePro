using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EZRide_Project.Model.Entities
{
    public class DriverDocuments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocumentId { get; set; }

        public int DriverId { get; set; } // FK → Drivers

        [Required]
        [Column(TypeName = "varchar(50)")]
        public DocumentTypes DocumentType { get; set; } // Enum: License, IDProof, AddressProof

        [Required]
        [Column(TypeName = "varchar(500)")]
        public string DocumentPath { get; set; }

        [Required]
        [Column(TypeName = "varchar(20)")]
        public DocumentStatus Status { get; set; } // Enum: Pending, Verified, Rejected

        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "varchar(500)")]
        public string? PublicId { get; set; }


        // Navigation property
        public Driver Driver { get; set; }

        public enum DocumentTypes
        {
            License,
            IDProof,
            AddressProof
        }

        public enum DocumentStatus
        {
            Pending,
            Verified,
            Rejected
        }
    }

}
