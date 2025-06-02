namespace EZRide_Project.DTO
{
    public class BookingFilterDTO
    {
        public string? BookingStatus { get; set; }      // "Completed", "Pending", etc.
        public string? PaymentStatus { get; set; }      // "Success", "Failed", etc.
        public string? SortBy { get; set; }             // "latest" / "old"

        public string? VehicleType { get; set; }
        public int? MinDays { get; set; }               // Optional
        public int? MinHours { get; set; }              // Optional
        public int? MinKilometers { get; set; }
        public bool? OnlyToday { get; set; }// Optional
    }
}
