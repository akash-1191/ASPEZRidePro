namespace EZRide_Project.DTO.Vehile_Owner_DTo
{
    public class OwnerPaymentDto
    {
        public int OwnerPaymentId { get; set; }
        public int UserId { get; set; }
        public int VehicleId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentType { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        // Vehicle Details
        public string VehicleName { get; set; }
        public string RegistrationNo { get; set; }
        public string VehicleType { get; set; }

        // Availability Details
        public int? AvailableDays { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public decimal? VehicleAmountPerDay { get; set; }
        public string AvailabilityStatus { get; set; }

        // Calculated fields
        public decimal? TotalRentAmount { get; set; } 
        public string? RentalPeriod { get; set; }
    }
}
