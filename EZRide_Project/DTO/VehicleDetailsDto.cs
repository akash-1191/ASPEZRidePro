namespace EZRide_Project.DTO
{
    public class VehicleDetailsDto
    {
        //show all vehicle related data

        // From Vehicles table
        public int VehicleId { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; }
        public string RegistrationNo { get; set; }
        public string Availability { get; set; }
        public string FuelType { get; set; }
        public byte SeatingCapacity { get; set; }
        public string TransmissionType { get; set; }
        public decimal Mileage { get; set; }
        public string Color { get; set; }
        public int YearOfManufacture { get; set; }
        public string InsuranceStatus { get; set; }
        public string RcStatus { get; set; }
        public string AcAvailability { get; set; }
        public decimal FuelTankCapacity { get; set; }
        public string CarType { get; set; }
        public int? EngineCapacity { get; set; }
        public string BikeType { get; set; }
        public DateTime CreatedAt { get; set; }

        // From PricingRules table
        public decimal PricePerKm { get; set; }
        public decimal PricePerHour { get; set; }
        public decimal PricePerDay { get; set; }

        // From VehicleImages table
        public List<string> ImagePaths { get; set; }
    }
}
