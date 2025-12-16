using EZRide_Project.Model.Entities;

namespace EZRide_Project.DTO.Driver_DTO
{
    public class UpsertDriverDto
    {
        public int ExperienceYears { get; set; }
        public string? AvailabilityStatus { get; set; }
        public Driver.VehicleType VehicleTypes { get; set; }
    }
}
