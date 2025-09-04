namespace EZRide_Project.DTO
{
    public class PaymentDto
    {
        public int BookingId { get; set; }
        public decimal Amount { get; set; }

        public string TransactionId { get; set; }   // Razorpay payment_id
        public string OrderId { get; set; }         // Razorpay order_id (optional)
        public string PaymentMethod { get; set; }   // "Online" or "Cash"
        public string Status { get; set; }          // "Success", "Failed", etc.
        public DateTime CreatedAt { get; set; }
    }
}
