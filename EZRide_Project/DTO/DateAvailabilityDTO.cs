namespace EZRide_Project.DTO
{
    public class DateAvailabilityDTO
    {
        public DateTime StartDateTime { get; set; }   // Changed from Date to DateTime
        public DateTime EndDateTime { get; set; }     // Optional: if frontend wants to show end time
        public bool IsAvailable { get; set; }
    }
}
