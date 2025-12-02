using EZRide_Project.DTO;
using EZRide_Project.Helpers;
using EZRide_Project.Model.Entities;
using EZRide_Project.Model;
using EZRide_Project.Repositories;

namespace EZRide_Project.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepo;
        private readonly IWebHostEnvironment _env;
        public VehicleService(IVehicleRepository vehicleRepo, IWebHostEnvironment env)
        {
            _env = env;
            _vehicleRepo = vehicleRepo;
        }

        public async Task<ApiResponseModel> AddVehicleAsync(VehicleCreateDTO dto, int userId)
        {
            try
            {
                var vehicle = new Vehicle
                {
                    UserId = userId, 
                    Vehicletype = Enum.Parse<Vehicle.VehicleType>(dto.Vehicletype, true),
                    RegistrationNo = dto.RegistrationNo,
                    Availability = Enum.Parse<Vehicle.AvailabilityStatus>(dto.Availability, true),
                    FuelType = Enum.Parse<Vehicle.Fueltype>(dto.FuelType, true),
                    SeatingCapacity = dto.SeatingCapacity,
                    Mileage = dto.Mileage,
                    Color = dto.Color,
                    YearOfManufacture = dto.YearOfManufacture,
                    InsuranceStatus = Enum.Parse<Vehicle.Insurancestatus>(dto.InsuranceStatus, true),
                    RcStatus = Enum.Parse<Vehicle.Rcstatus>(dto.RcStatus, true),
                    AcAvailability = Enum.TryParse<Vehicle.acAvailability>(dto.AcAvailability, true, out var acAvailEnum)
                        ? acAvailEnum : (Vehicle.acAvailability?)null,
                    FuelTankCapacity = dto.FuelTankCapacity,
                    CarName = dto.CarName,
                    EngineCapacity = dto.EngineCapacity,
                    BikeName = dto.BikeName,
                    CreatedAt = DateTime.Now,
                    SecurityDepositAmount = dto.SecurityDepositAmount,
                    Status = false
                };

                _vehicleRepo.AddVehicle(vehicle);
                var saved = await _vehicleRepo.SaveChangesAsync();

                if (!saved)
                    return ApiResponseHelper.Fail("Failed to add vehicle.");

                return ApiResponseHelper.Success("Vehicle added successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Fail("Exception: " + ex.Message);
            }
        }


        //Get all data of the vehicle table 
        public async Task<List<VehicleCreateDTO>> GetAllVehiclesAsync(int adminId)
        {
            var vehicles = await _vehicleRepo.GetAllVehiclesAsync(adminId);

            var dtoList = vehicles.Select(v => new VehicleCreateDTO
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
                SecurityDepositAmount =v.SecurityDepositAmount,
                EngineCapacity = v.EngineCapacity,
                BikeName = v.BikeName
            }).ToList();

            return dtoList;
        }

        //upadtdataa of the vehicle 

        public async Task<ApiResponseModel> UpdateVehicleAsync(VehicleCreateDTO dto, int userId)
        {
            var existingVehicle = await _vehicleRepo.GetVehicleByIdAsync(dto.VehicleId);

            if (existingVehicle == null)
                return ApiResponseHelper.Fail("Vehicle not found.");

            if (existingVehicle.UserId != userId)
                return ApiResponseHelper.Fail("You are not authorized to update this vehicle.");

            existingVehicle.Vehicletype = Enum.Parse<Vehicle.VehicleType>(dto.Vehicletype, true);
            existingVehicle.RegistrationNo = dto.RegistrationNo;
            existingVehicle.Availability = Enum.Parse<Vehicle.AvailabilityStatus>(dto.Availability, true);
            existingVehicle.FuelType = Enum.Parse<Vehicle.Fueltype>(dto.FuelType, true);
            existingVehicle.SeatingCapacity = dto.SeatingCapacity;
            existingVehicle.Mileage = dto.Mileage;
            existingVehicle.Color = dto.Color;
            existingVehicle.YearOfManufacture = dto.YearOfManufacture;
            existingVehicle.InsuranceStatus = Enum.Parse<Vehicle.Insurancestatus>(dto.InsuranceStatus, true);
            existingVehicle.RcStatus = Enum.Parse<Vehicle.Rcstatus>(dto.RcStatus, true);
            existingVehicle.AcAvailability = Enum.TryParse<Vehicle.acAvailability>(dto.AcAvailability, true, out var acAvailEnum)
                ? acAvailEnum : (Vehicle.acAvailability?)null;
            existingVehicle.FuelTankCapacity = dto.FuelTankCapacity;
            existingVehicle.CarName = dto.CarName;
            existingVehicle.EngineCapacity = dto.EngineCapacity;
            existingVehicle.BikeName = dto.BikeName;
            existingVehicle.SecurityDepositAmount = dto.SecurityDepositAmount;

            _vehicleRepo.UpdateVehicle(existingVehicle);
            var saved = await _vehicleRepo.SaveChangesAsync();

            if (!saved)
                return ApiResponseHelper.Fail("Failed to update vehicle.");

            return ApiResponseHelper.Success("Vehicle updated successfully.");
        }

        //deletedata of the vehicle table
        public async Task<ApiResponseModel> DeleteVehicleAsync(int vehicleId)
        {
            try
            {
                var isDeleted = await _vehicleRepo.DeleteVehicleAsync(vehicleId);
                if (isDeleted)
                {
                    return ApiResponseHelper.Success("Vehicle deleted successfully.");
                }
                else
                {
                    return ApiResponseHelper.NotFound("Vehicle not found.");
                }
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Fail("Exception: " + ex.Message);
            }
        }


    }
}
