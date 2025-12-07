using EZRide_Project.DTO.Vehile_Owner_DTo;
using EZRide_Project.Helpers;
using EZRide_Project.Model;
using EZRide_Project.Model.Entities;
using EZRide_Project.Repositories;
using static EZRide_Project.Services.OwnerVehicleService;

namespace EZRide_Project.Services
{
    public class OwnerVehicleService : IOwnerVehicleService
    {
        private readonly IOwnerVehicleRepository _ownerRepo;

        public OwnerVehicleService(IOwnerVehicleRepository ownerRepo)
        {
            _ownerRepo = ownerRepo;
        }

        public async Task<List<VehicleDTO>> GetOwnerVehiclesAsync(int ownerId)
        {
            var vehicles = await _ownerRepo.GetVehiclesByOwnerAsync(ownerId);


            return vehicles.Select(v =>
            {
                var lastAvailability = v.OwnerVehicleAvailabilities
                    .Where(a => a.Status == OwnerVehicleAvailability.AvailabilityStatus.Active)
                    .OrderByDescending(a => a.CreatedAt)
                    .FirstOrDefault();

                return new VehicleDTO
                {
                    VehicleId = v.VehicleId,
                    Vehicletype = v.Vehicletype.ToString(),
                    RegistrationNo = v.RegistrationNo,
                    Availability = v.Availability.ToString(),
                    FuelType = v.FuelType.ToString(),
                    SeatingCapacity = v.SeatingCapacity,
                    Mileage = v.Mileage,
                    Color = v.Color,
                    YearOfManufacture = v.YearOfManufacture,
                    InsuranceStatus = v.InsuranceStatus.ToString(),
                    RcStatus = v.RcStatus.ToString(),
                    AcAvailability = v.AcAvailability?.ToString(),
                    FuelTankCapacity = v.FuelTankCapacity,
                    CarName = v.CarName,
                    EngineCapacity = v.EngineCapacity,
                    BikeName = v.BikeName,
                    SecurityDepositAmount = v.SecurityDepositAmount,
                    Status = v.Status,

                    // NEW — From Availability Table
                    AvailableDays = lastAvailability?.AvailableDays??0,
                    EffectiveFrom = lastAvailability?.EffectiveFrom ?? default(DateTime),
                    EffectiveTo = lastAvailability?.EffectiveTo ?? default(DateTime)
                };

            }).ToList();
        }


        public async Task<ApiResponseModel> UpdateOwnerVehicleAsync(VehicleDTO dto, int ownerId)
        {
            var vehicle = await _ownerRepo.GetVehicleByIdAsync(dto.VehicleId);

            if (vehicle == null)
                return ApiResponseHelper.Fail("Vehicle not found.");

            if (vehicle.UserId != ownerId)
                return ApiResponseHelper.Fail("You are not authorized to update this vehicle.");

            if (vehicle.Status == true)
                return ApiResponseHelper.Fail("This vehicle is currently active/working and cannot be updated. Please contact admin.");

            //  Owner cannot update price & security deposit
            dto.SecurityDepositAmount = vehicle.SecurityDepositAmount;

            // Allowed updates only
            vehicle.Vehicletype = Enum.Parse<Vehicle.VehicleType>(dto.Vehicletype, true);
            vehicle.RegistrationNo = dto.RegistrationNo;
            vehicle.Availability = Enum.Parse<Vehicle.AvailabilityStatus>(dto.Availability, true);
            vehicle.FuelType = Enum.Parse<Vehicle.Fueltype>(dto.FuelType, true);
            vehicle.SeatingCapacity = dto.SeatingCapacity;
            vehicle.Mileage = dto.Mileage;
            vehicle.Color = dto.Color;
            vehicle.YearOfManufacture = dto.YearOfManufacture;
            vehicle.InsuranceStatus = Enum.Parse<Vehicle.Insurancestatus>(dto.InsuranceStatus, true);
            vehicle.RcStatus = Enum.Parse<Vehicle.Rcstatus>(dto.RcStatus, true);
            vehicle.AcAvailability = Enum.TryParse<Vehicle.acAvailability>(dto.AcAvailability, true, out var acAvailEnum)
                    ? acAvailEnum : (Vehicle.acAvailability?)null;
            vehicle.FuelTankCapacity = dto.FuelTankCapacity;
            vehicle.CarName = dto.CarName;
            vehicle.EngineCapacity = dto.EngineCapacity;
            vehicle.BikeName = dto.BikeName;

            _ownerRepo.UpdateVehicle(vehicle);
            var saved = await _ownerRepo.SaveChangesAsync();

            return saved
                ? ApiResponseHelper.Success("Vehicle updated successfully.")
                : ApiResponseHelper.Fail("Failed to update vehicle.");
        }


        // DELETE Vehicle By Owner
        public async Task<ApiResponseModel> DeleteOwnerVehicleAsync(int vehicleId, int ownerId)
        {
            try
            {
                var vehicle = await _ownerRepo.GetVehicleByIdAsync(vehicleId);

                if (vehicle == null)
                    return ApiResponseHelper.Fail("Vehicle not found.");

                if (vehicle.UserId != ownerId)
                    return ApiResponseHelper.Fail("You are not authorized to delete this vehicle.");

                if (vehicle.Status == true)
                    return ApiResponseHelper.Fail("This vehicle is currently active/working and cannot be deleted. Please contact admin.");

                _ownerRepo.DeleteVehicle(vehicle);
                var saved = await _ownerRepo.SaveChangesAsync();

                if (saved)
                    return ApiResponseHelper.Success("Vehicle deleted successfully.");

                return ApiResponseHelper.Fail("Failed to delete vehicle.");
            }
            catch (Exception ex)
            {
                // backend crash error also return clean message
                return ApiResponseHelper.Fail("Server error: " + ex.Message);
            }
        }


        //add owner vehile avalibilityes days
        public async Task<string> AddAvailabilityAsync(AddAvailabilityDto dto, int ownerId)
        {
            // Step 1 — Check if vehicle belongs to owner
            var isOwner = await _ownerRepo.IsVehicleOwnedByUserAsync(dto.VehicleId, ownerId);
            if (!isOwner)
                return "Unauthorized: This vehicle does not belong to you.";

            // Step 2 — Get vehicle info
            var vehicle = await _ownerRepo.GetVehicleByIdAsync(dto.VehicleId);
            if (vehicle == null)
                return "Vehicle not found.";

            // Step 3 — Check running status
            if (vehicle.Status == false)   // status = false → running
                return "This vehicle is currently running. Availability cannot be changed.";

            // Step 4 — Check if availability already exists
            var existing = await _ownerRepo.GetAvailabilityByVehicleAndOwnerAsync(dto.VehicleId, ownerId);

            if (existing == null)
            {
                // INSERT
                var availability = new OwnerVehicleAvailability
                {
                    VehicleId = dto.VehicleId,
                    OwnerId = ownerId,
                    AvailableDays = dto.AvailableDays,
                    EffectiveFrom = dto.EffectiveFrom,
                    EffectiveTo = dto.EffectiveTo,
                    Status = OwnerVehicleAvailability.AvailabilityStatus.Active,
                    CreatedAt = DateTime.Now
                };

                await _ownerRepo.AddAvailabilityAsync(availability);
                return "Availability added successfully!";
            }
            else
            {
                // UPDATE
                existing.AvailableDays = dto.AvailableDays;
                existing.EffectiveFrom = dto.EffectiveFrom;
                existing.EffectiveTo = dto.EffectiveTo;

                await _ownerRepo.UpdateAvailabilityAsync(existing);
                return "Availability updated successfully!";
            }
        }
    }
}

