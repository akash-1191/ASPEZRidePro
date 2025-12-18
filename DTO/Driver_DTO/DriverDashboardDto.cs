namespace EZRide_Project.DTO.Driver_DTO
{
    public class DriverDashboardDto
    {
        public string DriverName { get; set; }
        public DateTime TodayDate { get; set; }
        public int TotalTrips { get; set; }
        public int TodaysTrips { get; set; }
        public object OngoingTrip { get; set; }
        public decimal TotalEarnings { get; set; }
    }
}
