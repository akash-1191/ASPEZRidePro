using EZRide_Project.DTO;
using EZRide_Project.Helpers;
using EZRide_Project.Model;
using EZRide_Project.Model.Entities;
using EZRide_Project.Repositories;

namespace EZRide_Project.Services
{
    public class PricingRuleService : IPricingRuleService
    {
        private readonly IPricingRuleRepository _repository;

        public PricingRuleService(IPricingRuleRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponseModel> SetOrUpdatePricingAsync(PricingRuleDto dto)
        {
            try
            {
                var existing = await _repository.GetByVehicleIdAsync(dto.VehicleId);

                if (existing == null)
                {
                    var newPricing = new PricingRule
                    {
                        VehicleId = dto.VehicleId,
                        CreatedAt = DateTime.UtcNow
                    };

                    SetPrice(newPricing, dto);

                    await _repository.AddAsync(newPricing);
                    await _repository.SaveChangesAsync();

                    return ApiResponseHelper.Success("Pricing added successfully", newPricing);
                }
                else
                {
                    SetPrice(existing, dto);
                    await _repository.UpdateAsync(existing);
                    await _repository.SaveChangesAsync();

                    return ApiResponseHelper.Success("Pricing updated successfully", existing);
                }
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.ServerError("An error occurred while saving pricing.");
            }
        }

        private void SetPrice(PricingRule pricingRule, PricingRuleDto dto)
        {
            pricingRule.PricePerKm = dto.PricePerKm;
            pricingRule.PricePerHour = dto.PricePerHour;
            pricingRule.PricePerDay = dto.PricePerDay;
        }



        //get all data
        public async Task<ApiResponseModel> GetPricingByVehicleIdAsync(int vehicleId)
        {
            var pricing = await _repository.GetByVehicleIdAsync(vehicleId);

            if (pricing == null)
            {
                return ApiResponseHelper.NotFound("Pricing data for vehicle");
            }

            var dto = new PricingRuleResponseDto
            {
                PricingId = pricing.PricingId,
                VehicleId = pricing.VehicleId,
                PricePerKm = pricing.PricePerKm,
                PricePerHour = pricing.PricePerHour,
                PricePerDay = pricing.PricePerDay,
                CreatedAt = pricing.CreatedAt
            };

            return ApiResponseHelper.Success("Pricing data fetched", dto);
        }
    }
}
