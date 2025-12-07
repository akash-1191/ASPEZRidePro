namespace EZRide_Project.DTO.Vehile_Owner_DTo
{
    public class AddAvailabilityDto
    {
        public int VehicleId { get; set; }
        public int AvailableDays { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime EffectiveTo { get; set; }
        public bool? Status { get; set; }
    }
}
