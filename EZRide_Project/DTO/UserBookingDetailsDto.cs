namespace EZRide_Project.DTO
{
    public class UserBookingDetailsDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public List<VehicleDto> Vehicles { get; set; }
        public List<BookingDto> Bookings { get; set; }
    }

    public class VehicleDto
    {
        public int VehicleId { get; set; }
        public string RegistrationNo { get; set; }
        public string VehicleType { get; set; }
        public string Availability { get; set; }
        public string Color { get; set; }
        public int YearOfManufacture { get; set; }
    }

    public class BookingDto
    {
        public int BookingId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }

        // ✅ Corrected type
        public PaymentDetailDto? Payment { get; set; }

        public SecurityDepositDto? SecurityDeposit { get; set; }
    }

    public class PaymentDetailDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string TransactionId { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
    }

    public class SecurityDepositDto
    {
        public int DepositId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime? RefundedAt { get; set; }
    }
}
