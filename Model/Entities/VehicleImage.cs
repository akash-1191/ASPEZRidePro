using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EZRide_Project.Model.Entities
{
    public class VehicleImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VehicleImageId { get; set; }
        public int VehicleId { get; set; } 

        [Required]
        [Column(TypeName ="varchar(100)")]
        public string ImagePath { get; set; } 
        public DateTime CreatedAt { get; set; } 

        // Navigation property for Vehicle
        public Vehicle Vehicle { get; set; }

    }
}
