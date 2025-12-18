namespace EZRide_Project.DTO.Driver_DTO
{
    public class DriverCreateOrderDto
    {
        public int DriverId { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
    }
}
