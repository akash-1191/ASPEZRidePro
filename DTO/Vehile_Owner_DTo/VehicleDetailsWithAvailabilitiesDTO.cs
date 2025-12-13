namespace EZRide_Project.DTO.Vehile_Owner_DTo
{
    namespace EZRide_Project.DTO
    {
        // Better name for GET response
        public class VehicleDetailsWithAvailabilitiesDTO
        {
            // Vehicle details
            public int VehicleId { get; set; }
            public string Vehicletype { get; set; }
            public string RegistrationNo { get; set; }
            public string Availability { get; set; }
            public string FuelType { get; set; }
            public byte? SeatingCapacity { get; set; }
            public decimal Mileage { get; set; }
            public string Color { get; set; }
            public int YearOfManufacture { get; set; }
            public string InsuranceStatus { get; set; }
            public string RcStatus { get; set; }
            public string? AcAvailability { get; set; }
            public decimal FuelTankCapacity { get; set; }
            public string? CarName { get; set; }
            public int EngineCapacity { get; set; }
            public string? BikeName { get; set; }
            public bool? Status { get; set; }
            public decimal? SecurityDepositAmount { get; set; }
            public bool? IsApproved { get; set; }
            public string? RejectReason { get; set; }

            // Current active availability
            public int? AvailabilityId { get; set; }
            public decimal? VehicleAmountPerDay { get; set; }

            public List<AvailabilityDetailDTO> AllAvailabilities { get; set; } = new();
        }

        public class AvailabilityDetailDTO
        {
            public int AvailabilityId { get; set; }
            public int AvailableDays { get; set; }
            public DateTime EffectiveFrom { get; set; }
            public DateTime EffectiveTo { get; set; }
            public string Status { get; set; } // "Active" or "Expired"
            public decimal VehicleAmountPerDay { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}
