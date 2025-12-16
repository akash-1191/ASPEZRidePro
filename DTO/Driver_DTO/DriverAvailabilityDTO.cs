namespace EZRide_Project.DTO.Driver_DTO
{
    public class DriverAvailabilityDTO
    {
        public int DriverId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Phone { get; set; }
        public string VehicleType { get; set; } // 2Wheeler or 4Wheeler
        public int ExperienceYears { get; set; }
        public string Status { get; set; } // Active or Inactive
        public string AvailabilityStatus { get; set; } // Avai
    }
}
