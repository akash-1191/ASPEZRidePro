namespace EZRide_Project.DTO.Vehile_Owner_DTo
{
    public class OwnerDashboardDto
    {
        public int TotalVehicles { get; set; }
        public int ApprovedVehicles { get; set; }
        public int PendingVehicles { get; set; }
        public decimal TotalEarnings { get; set; }

        public int ReRentCount { get; set; }

    }
}
