using EZRide_Project.DTO.Vehile_Owner_DTo;
using EZRide_Project.Helpers;
using EZRide_Project.Model;
using EZRide_Project.Model.Entities;
using EZRide_Project.Repositories;

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

            return vehicles.Select(v => new VehicleDTO
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
                Status=v.Status
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




    }
}

