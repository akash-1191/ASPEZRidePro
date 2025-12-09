using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EZRide_Project.Model.Entities
{
    public class ChatMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MessageId { get; set; }

        public int ConversationId { get; set; } // FK → Conversations
        public int SenderId { get; set; } // FK → Users

        [Column(TypeName = "text")]
        public string MessageText { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime Timestamp { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public MessageStatus Status { get; set; } // Enum

        // Navigation properties
        [JsonIgnore]
        public Conversation Conversation { get; set; }
        public User Sender { get; set; }

        public enum MessageStatus
        {
            Sent,
            Delivered,
            Read
        }
    }
}
