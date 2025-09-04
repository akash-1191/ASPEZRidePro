namespace EZRide_Project.DTO
{
    public class DamageReportCreateDto
    {
        public int BookingId { get; set; }
        public string? Description { get; set; }
        public decimal? RepairCost { get; set; }
        public string? Image { get; set; }
        public string? Status { get; set; }
    }
}
