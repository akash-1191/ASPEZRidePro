using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EZRide_Project.Model.Entities
{
    public class Contact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContactId { get; set; }

        //public int? UserId { get; set; } // Foreign key for User

        [Column(TypeName ="Varchar(100)")]
        public string Subject { get; set; }

        [Column(TypeName ="Varchar(255)")]

        public string Email { get; set; }

        public string Phone {  get; set; } //phone number
        public string Message { get; set; }
        public ContactStatus Status { get; set; } // Enum can be used here
        public DateTime CreatedAt { get; set; }


        public enum ContactStatus
        {
            Open,
            Closed,
            Pending
        }
      
    }
}
