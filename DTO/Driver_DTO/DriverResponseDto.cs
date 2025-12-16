using EZRide_Project.Model.Entities;

namespace EZRide_Project.DTO.Driver_DTO
{
    public class DriverResponseDto
    {
        public int DriverId { get; set; }
        public int UserId { get; set; }
        public int ExperienceYears { get; set; }
        public string AvailabilityStatus { get; set; }
        public string Status { get; set; }
        public Driver.VehicleType VehicleType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
