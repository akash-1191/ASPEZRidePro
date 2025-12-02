using EZRide_Project.Data;
using EZRide_Project.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace EZRide_Project.Repositories
{
    public class OwnerVehicleRepository : IOwnerVehicleRepository
    {
        private readonly ApplicationDbContext _context;

        public OwnerVehicleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Vehicle>> GetVehiclesByOwnerAsync(int ownerId)
        {
            return await _context.Vehicles
                .Where(v => v.UserId == ownerId)
                .ToListAsync();
        }

        public async Task<Vehicle> GetVehicleByIdAsync(int vehicleId)
        {
            return await _context.Vehicles.FirstOrDefaultAsync(v => v.VehicleId == vehicleId);
        }

        public void UpdateVehicle(Vehicle vehicle)
        {
            _context.Vehicles.Update(vehicle);
        
        }

        public void DeleteVehicle(Vehicle vehicle)
        {
            _context.Vehicles.Remove(vehicle);
        }
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }





        //add vehicle avliblity
    }
}