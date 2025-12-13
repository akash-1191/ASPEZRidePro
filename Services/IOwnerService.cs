using EZRide_Project.DTO;
using EZRide_Project.DTO.Vehile_Owner_DTo;
using EZRide_Project.DTO.Vehile_Owner_DTo.EZRide_Project.DTO;
using EZRide_Project.Model;

namespace EZRide_Project.Services
{
    public interface IOwnerService
    {
        Task<List<OwnerApprovalDTO>> GetPendingOwnersAsync();
        ApiResponseModel ApproveOwner(int userId);
        ApiResponseModel RejectOwner(int userId, string reason);
        //ApiResponseModel GetPendingOwners();
        //Task<List<VehicleCreateDTO>> GetAllOwnerVehiclesAsync(int ownerId);
        Task<List<VehicleDetailsWithAvailabilitiesDTO>> GetAllOwnerVehiclesAsync(int ownerId);
        Task<List<ActiveOwnerDTO>> GetAllActiveOwnersAsync();
        Task<ApiResponseModel> AddOrUpdateDepositAsync(VehicleSecurityDepositDTO dto);

        Task<string> ApproveVehicleAsync(int vehicleId);
        Task<string> RejectVehicleAsync(int vehicleId, string reason);

        Task<string> UpdatePriceAsync(int availabilityId, decimal vehicleAmountPerDay);
          Task<List<OwnerPaymentInfoDto>> GetOwnerPaymentDataAsync();

    }
}
