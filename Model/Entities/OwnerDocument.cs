using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EZRide_Project.Model.Entities
{
    public class OwnerDocument
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocumentId { get; set; }

        // FK → Users(user_id)
        [Required]
        public int OwnerId { get; set; }
        public User Owner { get; set; }   // Navigation Property

        // Document Type (ENUM)
        [Required]
        [Column(TypeName = "varchar(250)")]
        public documentType DocumentType { get; set; }

        // File Path
        [Required]
        [MaxLength(255)]
        [Column(TypeName = "varchar(500)")]
        public string DocumentPath { get; set; }

        // Document Status
        [Required]
        [Column(TypeName = "varchar(20)")]
        public DocumentStatus Status { get; set; } = DocumentStatus.Pending;

        [Column(TypeName = "varchar(500)")]
        public string? PublicId { get; set; }


        // cancelation Reasion 

        [Column(TypeName = "varchar(20)")]
        public string? Reason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;


        // Enum for Document Type
        public enum documentType
        {
            RCBook,
            InsurancePaper,
            AadharCard
        }

        // Enum for Status
        public enum DocumentStatus
        {
            Pending,
            Verified,
            Rejected
        }
    }
}

