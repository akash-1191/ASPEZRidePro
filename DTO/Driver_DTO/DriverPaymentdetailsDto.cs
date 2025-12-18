namespace EZRide_Project.DTO.Driver_DTO
{
    public class DriverPaymentdetailsDto
    {
        public DriverDto Driver { get; set; }
        public BookingDto Booking { get; set; }
        public DriverPaymentInfoDto DriverPayment { get; set; }
    }

    public class DriverDto
    {
        public int DriverId { get; set; }
        public string DriverName { get; set; }
        public string DriverPhone { get; set; }
        public string DriverEmail { get; set; }
    }

    public class BookingDto
    {
        public int BookingId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; }
    }

    public class DriverPaymentInfoDto
    {
        public int DriverPaymentId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentType { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
