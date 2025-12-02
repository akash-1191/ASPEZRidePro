using EZRide_Project.Data;
using EZRide_Project.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace EZRide_Project.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly ApplicationDbContext _context;

        public VehicleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddVehicle(Vehicle vehicle)
        {
            _context.Vehicles.Add(vehicle);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        //getall data vehicle list
        public async Task<List<Vehicle>> GetAllVehiclesAsync(int adminId)
        {
            return await _context.Vehicles
                .Where(v=>v.UserId==adminId)
                .ToListAsync();
        }


        //update vehicle list
        public async Task<Vehicle?> GetVehicleByIdAsync(int vehicleId)
        {
            return await _context.Vehicles.FirstOrDefaultAsync(v => v.VehicleId == vehicleId);
        }

        public void UpdateVehicle(Vehicle vehicle)
        {
            _context.Vehicles.Update(vehicle);
        }

        // Delete data of the vehicle table
        public async Task<bool> DeleteVehicleAsync(int vehicleId)
        {
            var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.VehicleId == vehicleId);
            if (vehicle == null)
            {
                return false; 
            }

            _context.Vehicles.Remove(vehicle);
            int rowsAffected = await _context.SaveChangesAsync(); 
            return rowsAffected > 0; 
        }
        

    }

}
