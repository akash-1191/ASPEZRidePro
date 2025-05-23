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
            switch (dto.PriceType?.ToLower())
            {
                case "price_per_km":
                    pricingRule.PricePerKm = dto.Price;
                    pricingRule.PricePerHour = null;
                    pricingRule.PricePerDay = null;
                    break;
                case "price_per_hour":
                    pricingRule.PricePerHour = dto.Price;
                    pricingRule.PricePerKm = null;
                    pricingRule.PricePerDay = null;
                    break;
                case "price_per_day":
                    pricingRule.PricePerDay = dto.Price;
                    pricingRule.PricePerKm = null;
                    pricingRule.PricePerHour = null;
                    break;
            }
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
