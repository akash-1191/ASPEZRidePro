namespace EZRide_Project.DTO
{
    public class PricingRuleDto
    {
        public int VehicleId { get; set; }
        public decimal? PricePerKm { get; set; }
        public decimal? PricePerHour { get; set; }
        public decimal? PricePerDay { get; set; }
    }
}
