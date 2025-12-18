namespace EZRide_Project.DTO.Driver_DTO
{
    public class DriverPaymentDTO
    {
        public int DriverPaymentId { get; set; }

        public int DriverId { get; set; }

        public int BookingId { get; set; }

        public decimal Amount { get; set; }

        public string PaymentType { get; set; } // cash / online

        public string Status { get; set; } // pending / paid

        public DateTime? PaidAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
