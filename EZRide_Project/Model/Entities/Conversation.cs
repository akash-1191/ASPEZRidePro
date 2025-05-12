using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EZRide_Project.Model.Entities
{
    public class Conversation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ConversationId { get; set; }
        public int UserId { get; set; } // FK

        [Required]
        [Column(TypeName ="varchar(255)")]
        public string Message { get; set; }

        [Required]
        [Column(TypeName ="varchar(50)")]
        public ConversationStatus Status { get; set; } // Enum can be used here
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public User User { get; set; }
   



        public enum ConversationStatus
        {
            Open,
            Closed
        }
     
    }
}
