namespace EZRide_Project.DTO
{
    public class BookingDTO
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int VehicleId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal TotalDistance { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }  // Enum as string
        public DateTime CreatedAt { get; set; }
    }
}
