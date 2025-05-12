    using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EZRide_Project.Model.Entities
{
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }


        [Column(TypeName = "varchar(70)")]
        public Rolename RoleName { get; set; } // Enum can be used here


        [Required]
        [MaxLength(150)]
        [Column(TypeName = "varchar(150)")]
        public string Description { get; set; }


        // Navigation property for Users
        public ICollection<User> Users { get; set; }


        public enum Rolename
        {
            Admin,
            Customer,
            OwnerVehicle
        }
      
    }
}
