using EZRide_Project.Data;
using EZRide_Project.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace EZRide_Project.Repositories
{
    public class PricingRuleRepository : IPricingRuleRepository
    {
        private readonly ApplicationDbContext _context;

        public PricingRuleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //add and update data
        //get all data of the price
        public async Task<PricingRule> GetByVehicleIdAsync(int vehicleId)
        {
            return await _context.PricingRules.FirstOrDefaultAsync(p => p.VehicleId == vehicleId);
        }
            

        public async Task AddAsync(PricingRule pricingRule)
        {
            await _context.PricingRules.AddAsync(pricingRule);
        }

        public async Task UpdateAsync(PricingRule pricingRule)
        {
            _context.PricingRules.Update(pricingRule);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }


       
       

    }
}
