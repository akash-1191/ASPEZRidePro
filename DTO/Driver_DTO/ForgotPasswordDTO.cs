using System.ComponentModel.DataAnnotations;

namespace EZRide_Project.DTO.Driver_DTO
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
