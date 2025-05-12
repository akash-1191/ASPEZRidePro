using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static EZRide_Project.Model.Entities.Vehicle;

namespace EZRide_Project.Model.Entities
{
    public class Vehicle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VehicleId { get; set; } 
        public int UserId { get; set; }

        [Required]
        [Column(TypeName ="varchar(50)")]
        public VehicleType Type { get; set; } // Enum: 'bike', 'car'

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string RegistrationNo { get; set; }

        [Required]
        [Column(TypeName ="varchar(50)")]
        public AvailabilityStatus Availability { get; set; } // Enum: 'available', 'booked', 'disabled'

        [Required]
        [Column(TypeName = "varchar(50)")]
        public Fueltype FuelType { get; set; } // Enum: 'petrol', 'diesel', 'electric', 'CNG'


        public byte SeatingCapacity { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal Mileage { get; set; }

        [Required]
        [Column(TypeName ="varchar(50)")]
        public string Color { get; set; }
        public int YearOfManufacture { get; set; }

        [Required]
        [Column(TypeName ="varchar(50)")]
        public Insurancestatus InsuranceStatus { get; set; } // Enum: 'active', 'expired'


        [Required]
        [Column(TypeName = "varchar(50)")]
        public Rcstatus RcStatus { get; set; } // Enum: 'active', 'expired'


        [Required]
        [Column(TypeName = "varchar(50)")]
        public acAvailability AcAvailability { get; set; } // Enum: 'yes', 'no'

       
        [Column(TypeName = "decimal(5,2)")]
        public decimal FuelTankCapacity { get; set; }


        [Required]
        [Column(TypeName = "varchar(50)")]
        public Cartype CarType { get; set; } // Car type: 'sedan', 'suv', 'hatchback', 'other'
        public int EngineCapacity { get; set; }


        [Required]
        [Column(TypeName = "varchar(80)")]
        public Biketype BikeType { get; set; } // Bike type: 'sports', 'cruiser', 'scooter', 'other'
        public DateTime CreatedAt { get; set; } 



        // Navigation properties
        public User User { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        public PricingRule PricingRule { get; set; }
        public ICollection<OwnerPayment> OwnerPayments { get; set; }

        // VehicleImages Navigation Property
        public ICollection<VehicleImage> VehicleImages { get; set; }


        public enum VehicleType
        {
            Bike,
            Car
        }

        public enum AvailabilityStatus
        {
            Available,
            Booked,
            Disabled
        }
        public enum Fueltype
        {
            Petrol,
            Diesel,
            Electric,
            CNG
        }
        public enum Insurancestatus
        {
            Active,
            Expired
        }
        public enum Rcstatus
        {
            Active,
            Expired
        }
        public enum acAvailability
        {
            Yes,
            No
        }
        public enum Cartype
        {
            Sedan,
            SUV,
            Hatchback,
            Other
        }

        public enum Biketype
        {
            Sports,
            Cruiser,
            Scooter,
            Other
        }
    }
}
