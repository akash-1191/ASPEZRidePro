using static EZRide_Project.Model.Entities.Booking;

namespace EZRide_Project.DTO
{
    public class BookingDetailDTO
    {
        public int BookingId { get; set; }
        public string VehicleImage { get; set; }
        public string VehicleType { get; set; }
        public string VehicleName { get; set; }
        public string RegistrationNo { get; set; }
        public string FuelType { get; set; }
        public DateTime BookingDateTime { get; set; }
        public DateTime DropOffDateTime { get; set; }
        public string BookingType { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMode { get; set; }
        public string TransactionId { get; set; }
        public string BookingStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Useremail { get; set; }
        public string DriverFirstName { get; set; }
        public string DriverLastName { get; set; }
        public string DriverExperience { get; set; }
        public string DriverProfileImage { get; set; }


    }

}
