using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EZRide_Project.Model.Entities
{
    public class CustomerDocument
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocumentId { get; set; }
        public int UserId { get; set; } // FK for User

        
        [Column(TypeName ="Varchar(150)")]
        public string? AgeProofPath { get; set; }

       
        [Column(TypeName = "Varchar(150)")]
        public string? AddressProofPath { get; set; }

        [Column(TypeName = "varchar(150)")]
        public string? DLImagePath { get; set; }

        [Column(TypeName ="varchar(50)")]
        public DocumentStatus Status { get; set; } = DocumentStatus.Active;  // Enum can be used here
        public DateTime CreatedAt { get; set; }

        // Navigation property for User
        public User User { get; set; }

            
        public enum DocumentStatus
        {
            Active,
            Disabled
        }
       


    }
}
