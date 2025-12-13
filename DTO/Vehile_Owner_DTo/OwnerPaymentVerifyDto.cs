namespace EZRide_Project.DTO.Vehile_Owner_DTo
{
    public class OwnerPaymentVerifyDto
    {
        public string RazorpayOrderId { get; set; }
        public string RazorpayPaymentId { get; set; }
        public string RazorpaySignature { get; set; }

        public int OwnerId { get; set; }
        public int VehicleId { get; set; }

        public decimal Amount { get; set; }
        public decimal AmountPerDay { get; set; }
        public int AvailableDays { get; set; }
    }
}
