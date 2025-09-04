namespace EZRide_Project.DTO
{
    public class PricingRuleResponseDto
    {
        public int PricingId { get; set; }
        public int VehicleId { get; set; }
        public decimal? PricePerKm { get; set; }
        public decimal? PricePerHour { get; set; }
        public decimal? PricePerDay { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
