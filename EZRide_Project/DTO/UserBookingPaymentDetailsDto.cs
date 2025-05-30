namespace EZRide_Project.DTO
{
    public class UserBookingPaymentDetailsDto
    {
        public string VehicleName { get; set; }
        public string VehicleType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal SecurityDepositAmount { get; set; }
        public string PaymentStatus { get; set; }

    }
}
