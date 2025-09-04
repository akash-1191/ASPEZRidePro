namespace EZRide_Project.DTO
{
    public class DateAvailabilityDTO
    {
        public DateTime StartDateTime { get; set; }   
        public DateTime EndDateTime { get; set; }     
        public bool IsAvailable { get; set; }
        public string Status { get; set; }
    }
}
