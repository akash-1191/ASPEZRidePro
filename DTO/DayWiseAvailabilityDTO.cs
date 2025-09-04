namespace EZRide_Project.DTO
{
    public class DayWiseAvailabilityDTO
    {
        public string Date { get; set; }               // "2024-06-05"
        public List<TimeRangeDTO> UnavailableSlots { get; set; } = new List<TimeRangeDTO>();
    }

    public class TimeRangeDTO
    {
        public string StartTime { get; set; }          // "14:00"
        public string EndTime { get; set; }            // "16:00"
    }
}
