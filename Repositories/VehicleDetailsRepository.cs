using EZRide_Project.Data;
using EZRide_Project.DTO;
using Microsoft.EntityFrameworkCore;

namespace EZRide_Project.Repositories
{
    public class VehicleDetailsRepository : IVehicleDetailsRepository
    {
        private readonly ApplicationDbContext _context;

        public VehicleDetailsRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        //get data perticular vehicle Id 
        public VehicleDetailsDto GetVehicleDetailsById(int vehicleId)
        {
            var vehicle = _context.Vehicles
                .Include(v => v.VehicleImages)
                .Include(v => v.PricingRule)
                .FirstOrDefault(v => v.VehicleId == vehicleId);


            if (vehicle == null) return null;

            var pricing = vehicle.PricingRule;
            var images = vehicle.VehicleImages.Select(i => i.ImagePath).ToList();

            return new VehicleDetailsDto
            {
                VehicleId = vehicle.VehicleId,
                UserId = vehicle.UserId,
                Type = vehicle.Vehicletype.ToString(),
                RegistrationNo = vehicle.RegistrationNo,
                Availability = vehicle.Availability.ToString(),
                FuelType = vehicle.FuelType.ToString(),
                SeatingCapacity =vehicle.SeatingCapacity ??0,    
                Mileage = vehicle.Mileage,
                Color = vehicle.Color,
                YearOfManufacture = vehicle.YearOfManufacture,
                InsuranceStatus = vehicle.InsuranceStatus.ToString(),
                RcStatus = vehicle.RcStatus.ToString(),
                AcAvailability = vehicle.AcAvailability?.ToString(),
                FuelTankCapacity = vehicle.FuelTankCapacity,
                CarType = vehicle.CarName,
                EngineCapacity = vehicle.EngineCapacity,
                BikeType = vehicle.BikeName,
                SecurityDepositAmount = vehicle.SecurityDepositAmount,
                CreatedAt = vehicle.CreatedAt,

                PricePerKm = pricing?.PricePerKm ?? 0,
                PricePerHour = pricing?.PricePerHour ?? 0,
                PricePerDay = pricing?.PricePerDay ?? 0,

                ImagePaths = images
            };
        }

        //get all data of the vehicle 
        public List<VehicleDetailsDto> GetAllVehicleDetails()
        {
            var vehicles = _context.Vehicles
                .Include(v => v.VehicleImages)
                .Include(v => v.PricingRule)
                .Where(v=>v.SecurityDepositAmount != null && v.SecurityDepositAmount >0)
                .ToList();

            var vehicleDetailsList = vehicles.Select(vehicle =>
            {
                var pricing = vehicle.PricingRule;
                var images = vehicle.VehicleImages.Select(i => i.ImagePath).ToList();

                return new VehicleDetailsDto
                {
                    VehicleId = vehicle.VehicleId,
                    UserId = vehicle.UserId,
                    Type = vehicle.Vehicletype.ToString(),
                    RegistrationNo = vehicle.RegistrationNo,
                    Availability = vehicle.Availability.ToString(),
                    FuelType = vehicle.FuelType.ToString(),
                    SeatingCapacity = vehicle.SeatingCapacity ?? 0,
                    Mileage = vehicle.Mileage,
                    Color = vehicle.Color,
                    YearOfManufacture = vehicle.YearOfManufacture,
                    InsuranceStatus = vehicle.InsuranceStatus.ToString(),
                    RcStatus = vehicle.RcStatus.ToString(),
                    AcAvailability = vehicle.AcAvailability?.ToString(),
                    FuelTankCapacity = vehicle.FuelTankCapacity,
                    CarType = vehicle.CarName,
                    EngineCapacity = vehicle.EngineCapacity,
                    BikeType = vehicle.BikeName,
                    SecurityDepositAmount = vehicle.SecurityDepositAmount,
                    CreatedAt = vehicle.CreatedAt,

                    PricePerKm = pricing?.PricePerKm ?? 0,
                    PricePerHour = pricing?.PricePerHour ?? 0,
                    PricePerDay = pricing?.PricePerDay ?? 0,

                    ImagePaths = images
                };
            }).ToList();

            return vehicleDetailsList;
        }

    }

}
