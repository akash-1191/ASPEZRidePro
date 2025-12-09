using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EZRide_Project.Model.Entities
{
    public class Conversation    
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ConversationId { get; set; }

        public int Participant1Id { get; set; } 
        public int Participant2Id { get; set; } 

        [Required]
        [Column(TypeName = "varchar(50)")]
        public ConversationType Type { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public ConversationStatus Status { get; set; } 

        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public User Participant1 { get; set; }
        public User Participant2 { get; set; }
        [JsonIgnore]
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
