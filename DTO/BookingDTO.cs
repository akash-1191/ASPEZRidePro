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
        public string BookingType { get; set; }
        public int? TotalDays { get; set; }
        public int? TotalHours { get; set; }
        public int? PerKelomeater { get; set; }
        public string DriverRequired { get; set; } // "yes" or "no"
        public int? DriverId { get; set; }


        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    
    }
}
