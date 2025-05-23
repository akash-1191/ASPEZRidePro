namespace EZRide_Project.DTO
{
    public class PricingRuleDto
    {
        public int VehicleId { get; set; }
        public string PriceType { get; set; }  // price_per_km, price_per_hour, price_per_day
        public decimal Price { get; set; }
    }
}
