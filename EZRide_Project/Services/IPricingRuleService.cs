using EZRide_Project.DTO;
using EZRide_Project.Model;

namespace EZRide_Project.Services
{
    public interface IPricingRuleService
    {
        Task<ApiResponseModel> SetOrUpdatePricingAsync(PricingRuleDto dto);

        Task<ApiResponseModel> GetPricingByVehicleIdAsync(int vehicleId);
    }

}
