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
                .Include(v => v.OwnerVehicleAvailabilities)
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

        public async Task<bool> IsVehicleOwnedByUserAsync(int vehicleId, int ownerId)
        {
            return await _context.Vehicles
                .AnyAsync(v => v.VehicleId == vehicleId && v.UserId == ownerId);
        }

        public async Task AddAvailabilityAsync(OwnerVehicleAvailability availability)
        {
            await _context.OwnerVehicleAvailabilities.AddAsync(availability);
            await _context.SaveChangesAsync();
        }

        public async Task<OwnerVehicleAvailability> GetAvailabilityByVehicleAndOwnerAsync(int vehicleId, int ownerId)
        {
            return await _context.OwnerVehicleAvailabilities
                .FirstOrDefaultAsync(a => a.VehicleId == vehicleId && a.OwnerId == ownerId);
        }

        public async Task UpdateAvailabilityAsync(OwnerVehicleAvailability availability)
        {
            _context.OwnerVehicleAvailabilities.Update(availability);
            await _context.SaveChangesAsync();
        }

    }
}