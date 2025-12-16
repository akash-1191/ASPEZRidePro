namespace EZRide_Project.DTO.Driver_DTO
{
    public class OngoingTripDTO
    {
        public string DriverFullName { get; set; }
        public string DriverPhone { get; set; }
        public string VehicleType { get; set; }
        public string BikeName { get; set; }
        public string CarName { get; set; }
        public string RegistrationNo{ get; set; }

        public string CustomerFullName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerCity { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int DriverId { get; set; }
        public string UesrEmail { get; set; }
        public string DriverEmail { get; set; }
        public string Status { get; set; }
        public int DriverBookingId { get; set; }


        public string DriverAvailabiliStatus { get; set; } // Available,Busy,Inactive
    }

}
