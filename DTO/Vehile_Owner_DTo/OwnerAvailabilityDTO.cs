namespace EZRide_Project.DTO.Vehile_Owner_DTo
{
    public class OwnerAvailabilityDTO
    {
        public int VehicleId { get; set; }
        public int AvailableDays { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime EffectiveTo { get; set; }
        public decimal PricePerDay { get; set; }
    }
}
