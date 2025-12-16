namespace EZRide_Project.DTO.Driver_DTO
{
    public class DriverBooking
    {
        public int BookingId { get; set; }
        public int DriverId { get; set; }
        public int VehicleId { get; set; }
        public DateTime AssignTime { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
