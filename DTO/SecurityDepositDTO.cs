namespace EZRide_Project.DTO
{
    public class SecurityDepositDTO
    {
        public int DepositId { get; set; }
        public int BookingId { get; set; }

        public decimal Amount { get; set; }

        public string Status { get; set; } = "Confirmed"; // Default value

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? RefundedAt { get; set; }

        public string? VehicleName { get; set; }
        public string? RegistrationNo { get; set; }
        public string? Bookingstatus { get; set; }

    }
}
