namespace EZRide_Project.DTO
{
    public class PaymentReceiptDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string VehicleName { get; set; }
        public string VehicleType { get; set; }
        public string FuelType { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime DropDateTime { get; set; }
        public DateTime BookingCreatedAt { get; set; }
        public string BookingStatus { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionId { get; set; }
        public string PaymentStatus { get; set; }
        public decimal TotalPayment { get; set; }
        public decimal SecurityDepositAmount { get; set; }
        public string VehicleImagePath { get; set; }
    }
}
