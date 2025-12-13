namespace EZRide_Project.DTO.Vehile_Owner_DTo
{
    public class OwnerPaymentInfoDto
    {
        public int AvailabilityId { get; set; }
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }
        public int VehicleId { get; set; }
        public string RegistrationNo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        public decimal VehicleAmountPerDay { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
    }
}
