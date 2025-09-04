namespace EZRide_Project.DTO
{
    public class AdminUserOverallBookingInfoDto
    {

        // ========== Booking ==========
        public int? BookingId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? BookingStatus { get; set; } // Booking.Status (enum)
        public string? BookingType { get; set; }    // Booking.BookingType
        public int? TotalDays { get; set; }
        public int? TotalHours { get; set; }
        public int? PerKelomeater { get; set; }
        public DateTime? BookingCreatedAt { get; set; }



        // ========== Payment ==========
        public string? PaymentStatus { get; set; }  // Payment.Status
        public decimal? PaymentAmount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? TransactionId { get; set; }
        public string? OrderId { get; set; }
        public DateTime? PaymentCreatedAt { get; set; }

        // ========== Security Deposit ==========
        public string? SecurityDepositStatus { get; set; } // SecurityDeposit.Status
        public decimal? SecurityDepositAmount { get; set; }
        public DateTime? SecurityDepositCreatedAt { get; set; }
        public DateTime? RefundedAt { get; set; }

        // ========== User ==========
        public int? UserId { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public int? Age { get; set; }
        public string? Gender { get; set; }
        public string? UserImage { get; set; }
        public string? RoleName { get; set; } // User.Role.Name
        public DateTime? UserCreatedAt { get; set; }

        // ========== Vehicle ==========
        public int? VehicleId { get; set; }
        public string? VehicleType { get; set; } // Enum (Bike / Car)
        public string? RegistrationNo { get; set; }
        public string? FuelType { get; set; }
        public byte? SeatingCapacity { get; set; }
        public decimal? Mileage { get; set; }
        public string? Color { get; set; }
        public string? CarName { get; set; }
        public string? BikeName { get; set; }
        public string? Availability { get; set; } // Vehicle.Availability
        public string? InsuranceStatus { get; set; }
        public string? RcStatus { get; set; }
        public string? AcAvailability { get; set; }
        public decimal? FuelTankCapacity { get; set; }
        public int? YearOfManufacture { get; set; }
        public int? EngineCapacity { get; set; }
        public decimal? VehicleSecurityDepositAmount { get; set; }
        public DateTime? VehicleCreatedAt { get; set; }

        // ========== Vehicle Image (Optional Thumbnail) ==========
        public string? VehicleImage { get; set; } // Thumbnail
        public List<string>? VehicleImages { get; set; } // First image from VehicleImages


        // ===== Debug fields =====
        public string? DamageDescription { get; set; }
        public decimal? DamageCharge { get; set; }
        public string? DamageImage { get; set; }
        public string? DamageReportStatus { get; set; }

        public decimal? FuelGiven { get; set; }
        public decimal? FuelReturned { get; set; }
        public decimal? FuelCharge { get; set; }
        public string? FuelLogStatus { get; set; }
    }
}
