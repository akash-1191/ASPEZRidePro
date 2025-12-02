using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EZRide_Project.Model.Entities
{
    public class Conversation    
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ConversationId { get; set; }

        public int Participant1Id { get; set; } // FK → Users
        public int Participant2Id { get; set; } // FK → Users

        [Required]
        [Column(TypeName = "varchar(50)")]
        public ConversationType Type { get; set; } // Enum

        [Required]
        [Column(TypeName = "varchar(50)")]
        public ConversationStatus Status { get; set; } // Enum

        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public User Participant1 { get; set; }
        public User Participant2 { get; set; }
        public ICollection<ChatMessage> ChatMessages { get; set; }

        public enum ConversationType
        {
            AdminDriver,
            AdminOwner,
            CustomerDriver
        }

        public enum ConversationStatus
        {
            Open,
            Closed
        }
    }


}
