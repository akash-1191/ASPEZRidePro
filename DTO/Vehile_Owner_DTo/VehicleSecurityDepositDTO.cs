using System.ComponentModel.DataAnnotations;

namespace EZRide_Project.DTO.Vehile_Owner_DTo
{
    public class VehicleSecurityDepositDTO
    {
        [Required]
        public int VehicleId { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be greater than or equal to 0")]
        public decimal Amount { get; set; }
    }
}
