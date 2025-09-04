using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static EZRide_Project.Model.Entities.Feedback;

namespace EZRide_Project.Model.Entities
{
    public class Feedback
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FeedbackId { get; set; }
        public int UserId { get; set; } //FK

        [Column(TypeName ="varchar(50)")]
        public Feedbacktype FeedbackType { get; set; } // Enum can be used here

        [Column(TypeName ="varchar(255)")]
        public string Message { get; set; }

        [Column(TypeName = "varchar(50)")]
        public FeedbackStatus Status { get; set; } // Enum can be used here
        public DateTime CreatedAt { get; set; }

        // Navigation property for User
        public User User { get; set; }


        public enum Feedbacktype
        {
            Complaint,
            Suggestion,
            Review
        }
        public enum FeedbackStatus
        {
            Open,
            Resolved,
            Pending
        }
      
    }
}
