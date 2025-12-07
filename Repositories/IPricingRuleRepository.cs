using EZRide_Project.Model.Entities;

namespace EZRide_Project.Repositories
{
    public interface IPricingRuleRepository
    {

        //add and update price
        //get the all price
        Task<PricingRule> GetByVehicleIdAsync(int vehicleId);
        Task AddAsync(PricingRule pricingRule);
        Task UpdateAsync(PricingRule pricingRule);
        Task SaveChangesAsync();



         
        
    }
}
