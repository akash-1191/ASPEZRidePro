namespace EZRide_Project.DTO.Vehile_Owner_DTo
{
    public class CreateOwnerPaymentDto
    {
        public int AvailabilityId { get; set; }
        public decimal Amount { get; set; }
        public string RazorpayPaymentId { get; set; }
    }
}
