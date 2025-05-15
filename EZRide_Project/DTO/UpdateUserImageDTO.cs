using System.ComponentModel.DataAnnotations;

namespace EZRide_Project.DTO
{
    public class UpdateUserImageDTO
    {
        public int UserId { get; set; }
        [Required]
        public IFormFile Image { get; set; }
    }
}
