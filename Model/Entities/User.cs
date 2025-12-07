using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EZRide_Project.Model.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string Firstname { get; set; }


        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string? Middlename { get; set; }


        [Required]
        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string Lastname { get; set; }


        [Required]
        public int Age { get; set; }

        [Required]
        [Column(TypeName ="varchar(20)")]
        public string Gender { get; set; }


        [Required]
        [Column(TypeName = "varchar(255)")]
        public string Image { set; get; }


        [Required]
        [Column(TypeName = "varchar(50)")]
        public string City { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string State { get; set; }

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string Email { get; set; }

        [Required]
        [MaxLength(12)]
        [Column(TypeName = "varchar(15)")]
        public string Phone { get; set; }

        [Required]
        [Column(TypeName ="varchar(100)")]
        public string Password { get; set; }

        [Required]
        [Column(TypeName ="varchar(200)")]
        public string Address { get; set; }
        public UserStatus Status { get; set; } = UserStatus.Pending;

        [Column(TypeName = "varchar(200)")]
        public string? RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }

        // Foreign key for Role 
        public int RoleId { get; set; }
        public Role Role { get; set; }

        // Navigation properties
        public ICollection<Vehicle> Vehicles { get; set; }
        public ICollection<Booking> Bookings { get; set; } 
        public ICollection<OwnerPayment> OwnerPayments { get; set; }
        public ICollection<Conversation> ConversationsAsParticipant1 { get; set; }
        public ICollection<Conversation> ConversationsAsParticipant2 { get; set; }
        public ICollection<CustomerDocument> CustomerDocuments { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; }
        public ICollection<Contact> Contacts { get; set; }
        public ICollection<DriverReview> DriverReviews { get; set; }
        public Driver? Driver { get; set; }
        public ICollection<OwnerDocument> OwnerDocuments { get; set; }




        public enum UserStatus
        {
            Pending = 0,
            Active = 1,
            Disabled = 2
        }

    }
}
