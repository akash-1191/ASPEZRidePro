namespace EZRide_Project.DTO.Driver_DTO
{
    public class DriverPaymentVerifyDto
    {
        public int DriverId { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }

        public string RazorpayOrderId { get; set; }
        public string RazorpayPaymentId { get; set; }
        public string RazorpaySignature { get; set; }
    }
}
