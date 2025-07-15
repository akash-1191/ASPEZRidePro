namespace EZRide_Project.DTO
{
    public class FuelLogCreateDto
    {
        public int BookingId { get; set; }
        public decimal? FuelGiven { get; set; }
        public decimal? FuelReturned { get; set; }
        public decimal? FuelCharge { get; set; }
        public string? Status { get; set; }
    }
}
