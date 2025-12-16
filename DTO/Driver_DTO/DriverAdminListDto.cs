namespace EZRide_Project.DTO.Driver_DTO
{
    public class DriverAdminListDto
    {
        public int DriverId { get; set; }

        // User Info
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string Image { get; set; }
        public string UserStatus { get; set; }
        public string RejectResion { get; set; }


        // Driver Info
        public int ExperienceYears { get; set; }
        public string AvailabilityStatus { get; set; }
        public string VehicleType { get; set; }
        public string DriverStatus { get; set; }

        // Documents
        public List<DriverDocumentDto> Documents { get; set; }
    }
}
