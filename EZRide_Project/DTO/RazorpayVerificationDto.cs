namespace EZRide_Project.DTO
{
    public class RazorpayVerificationDto
    {

        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionId { get; set; }  // razorpay_payment_id
        public string OrderId { get; set; }        // razorpay_order_id
        public string Signature { get; set; }      // razorpay_signature
        public string PaymentMethod { get; set; } = "Online";
        public string Status { get; set; } = "Success";
        public DateTime CreatedAt { get; set; }
    }

    // For creating order (no bookingId needed here)
    public class CreateOrderRequestDto
    {
        public decimal Amount { get; set; }
    }
}
