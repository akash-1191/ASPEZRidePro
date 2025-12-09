using EZRide_Project.DTO;
using EZRide_Project.DTO.Vehile_Owner_DTo;
using EZRide_Project.Helpers;
using EZRide_Project.Model;
using EZRide_Project.Model.Entities;
using EZRide_Project.Repositories;
using NuGet.Protocol.Core.Types;

namespace EZRide_Project.Services
{
    public class OwnerService : IOwnerService
    {
        private readonly IOwnerRepository _ownerRepo;

        public OwnerService(IOwnerRepository ownerRepo)
        {
            _ownerRepo = ownerRepo;
        }

        public async Task<List<OwnerApprovalDTO>> GetPendingOwnersAsync()
        {
            return await _ownerRepo.GetPendingOwnersAsync();
        }


        public ApiResponseModel ApproveOwner(int userId)
        {
            var user = _ownerRepo.GetUserById(userId);
            if (user == null)
                return ApiResponseHelper.NotFound("Owner not found.");

            user.Status = User.UserStatus.Active;
            user.RejectionReason = null;

            _ownerRepo.UpdateUser(user);

            return ApiResponseHelper.Success("Owner approved successfully.");
        }

        //  Owner Reject + Reason
        public ApiResponseModel RejectOwner(int userId, string reason)
        {
            var user = _ownerRepo.GetUserById(userId);
            if (user == null)
                return ApiResponseHelper.NotFound("Owner not found.");

            if (string.IsNullOrWhiteSpace(reason))
                return ApiResponseHelper.Fail("Reason is required.");

            user.Status = User.UserStatus.Disabled;
            user.RejectionReason = reason;

            _ownerRepo.UpdateUser(user);

            return ApiResponseHelper.Success("Owner rejected successfully.");
        }

        //get aala vehole uploade by owner

        public async Task<List<VehicleCreateDTO>> GetAllOwnerVehiclesAsync(int ownerId)
        {
            var vehicles = await _ownerRepo.GetAllOwnerVehiclesAsync(ownerId);

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
                SecurityDepositAmount = v.SecurityDepositAmount,
                EngineCapacity = v.EngineCapacity,
                BikeName = v.BikeName,
                Status = v.Status,
                RejectReason = v.RejectReason,
                IsApproved = v.IsApproved,
                AvailabilityId = v.OwnerVehicleAvailabilities
    .FirstOrDefault(a => a.Status == OwnerVehicleAvailability.AvailabilityStatus.Active)
    ?.AvailabilityId ?? 0,

                VehicleAmountPerDay = v.OwnerVehicleAvailabilities
    .FirstOrDefault(a => a.Status == OwnerVehicleAvailability.AvailabilityStatus.Active)
    ?.vehicleAmountPerDay ?? 0

            }).ToList();

            return dtoList;
        }

        //get all aproval owner data
        public async Task<List<ActiveOwnerDTO>> GetAllActiveOwnersAsync()
        {
            var owners = await _ownerRepo.GetAllActiveOwnersAsync();

            return owners.Select(u => new ActiveOwnerDTO
            {
                UserId = u.UserId,
                Firstname = u.Firstname,
                Middlename = u.Middlename,
                Lastname = u.Lastname,
                Age = u.Age,
                Gender = u.Gender,
                Image = u.Image,
                City = u.City,
                State = u.State,
                Email = u.Email,
                Phone = u.Phone,
                Address = u.Address,
                Status = u.Status.ToString(),
                CreatedAt = u.CreatedAt
            }).ToList();
        }

        //add or update secutity deposit amount
        public async Task<ApiResponseModel> AddOrUpdateDepositAsync(VehicleSecurityDepositDTO dto)
        {
            try
            {
                var vehicle = await _ownerRepo.GetVehicleByIdAsync(dto.VehicleId);

                if (vehicle == null)
                    return ApiResponseHelper.NotFound("Vehicle not found.");

                if (vehicle.SecurityDepositAmount.HasValue)
                {
                    // Update existing deposit
                    vehicle.SecurityDepositAmount = dto.Amount;
                    await _ownerRepo.SaveChangesAsync();
                    return ApiResponseHelper.Success("Security deposit updated successfully.", dto);
                }
                else
                {
                    // Add new deposit
                    vehicle.SecurityDepositAmount = dto.Amount;
                    vehicle.CreatedAt = DateTime.UtcNow;
                    await _ownerRepo.SaveChangesAsync();
                    return ApiResponseHelper.Success("Security deposit added successfully.", dto);
                }
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.ServerError("An error occurred: " + ex.Message);
            }
        }




        // APPROVE VEHICLE
        // APPROVE VEHICLE
        public async Task<string> ApproveVehicleAsync(int vehicleId)
        {
            var vehicle = await _ownerRepo.GetVehicleByIdAsync(vehicleId);

            if (vehicle == null)
                return "Vehicle not found.";

            vehicle.IsApproved = true;
            vehicle.RejectReason = null;
            vehicle.Availability = Vehicle.AvailabilityStatus.Available;

            await _ownerRepo.UpdateVehicleAsync(vehicle);
            return "Vehicle approved successfully!";
        }

        // REJECT VEHICLE
        public async Task<string> RejectVehicleAsync(int vehicleId, string reason)
        {
            var vehicle = await _ownerRepo.GetVehicleByIdAsync(vehicleId);

            if (vehicle == null)
                return "Vehicle not found.";

            vehicle.IsApproved = false;
            vehicle.RejectReason = reason;
            vehicle.Availability = Vehicle.AvailabilityStatus.Disabled;

            await _ownerRepo.UpdateVehicleAsync(vehicle);
            return "Vehicle rejected successfully!";
        }


        public async Task<string> UpdatePriceAsync(int availabilityId, decimal vehicleAmountPerDay)
        {
            var availability = await _ownerRepo.GetAvailabilityByIdAsync(availabilityId);
            if (availability == null)
                return "Error: Availability not found.";

            availability.vehicleAmountPerDay = vehicleAmountPerDay;
            await _ownerRepo.UpdateAvailabilityAsync(availability);

            return "Price updated successfully!";
        }
    }
}
