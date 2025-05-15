using System.ComponentModel.DataAnnotations;

namespace EZRide_Project.DTO
{
    public class UserProfileUpdateDTO
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Firstname { get; set; }

        [MaxLength(100)]
        public string? Middlename { get; set; }

        [Required]
        [MaxLength(100)]
        public string Lastname { get; set; }

        [Required]
        [Range(1, 120)]
        public int Age { get; set; }

        [Required]
        [MaxLength(200)]
        public string Address { get; set; }

        [Required]
        [Phone]
        [MaxLength(11)]
        public string Phone { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        [MaxLength(50)]
        public string City { get; set; }

        [Required]
        [MaxLength(50)]
        public string State { get; set; }
    }
}
