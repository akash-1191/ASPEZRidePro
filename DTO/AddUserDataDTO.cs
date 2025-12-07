using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EZRide_Project.DTO
{
    public class AddUserDataDTO
    {
        [Required]
        public string Firstname { get; set; }
        public string? Middlename { get; set; }
        [Required]
        public string Lastname { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }

        [Required]
        public IFormFile Image { get; set; }
        public int RoleId { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? UserStatus { get; set; }
    }


}
