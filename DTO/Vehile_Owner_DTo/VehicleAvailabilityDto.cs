namespace EZRide_Project.DTO.Vehile_Owner_DTo
{
    public class VehicleAvailabilityDto
    {
        public int VehicleId { get; set; }
        public string OwnershipType { get; set; } // ADMIN / OWNER
        public bool IsUnlimited { get; set; }
        public DateTime? AvailableFrom { get; set; }
        public DateTime? AvailableTo { get; set; }
        public string Status { get; set; }
    }
}
