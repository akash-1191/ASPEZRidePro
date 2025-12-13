using System;
using EZRide_Project.Data;
using EZRide_Project.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace EZRide_Project.Repositories
{
    public class OwnerPaymentRepository : IOwnerPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public OwnerPaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OwnerPayment> AddPaymentAsync(OwnerPayment payment)
        {
            await _context.OwnerPayments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment;
        }



        // New method to update availability status
        public async Task<bool> UpdateVehicleAvailabilityStatusAsync(int vehicleId, int ownerId)
        {
            try
            {
                var activeAvailabilities = await _context.OwnerVehicleAvailabilities
                    .Where(av => av.VehicleId == vehicleId
                              && av.OwnerId == ownerId
                              && av.Status == OwnerVehicleAvailability.AvailabilityStatus.Active)
                    .ToListAsync();

                if (activeAvailabilities.Any())
                {
                    foreach (var availability in activeAvailabilities)
                    {
                        availability.Status = OwnerVehicleAvailability.AvailabilityStatus.Expired;
                        availability.EffectiveTo = DateTime.Now;
                        _context.OwnerVehicleAvailabilities.Update(availability);
                    }

                    await _context.SaveChangesAsync();
                    return true;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update availability status: {ex.Message}", ex);
            }
        }

        public async Task ClearSecurityDepositAmountAsync(int vehicleId)
        {
            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.VehicleId == vehicleId);

            if (vehicle != null)
            {
                vehicle.SecurityDepositAmount = null;
                _context.Vehicles.Update(vehicle);
                await _context.SaveChangesAsync();
            }
        }



        public async Task<List<OwnerPayment>> GetOwnerPaymentsAsync(int ownerId)
        {
            return await _context.OwnerPayments
        .Include(op => op.Vehicle)
        .ThenInclude(v => v.OwnerVehicleAvailabilities)
        .Where(x => x.UserId == ownerId )
        .OrderByDescending(x => x.CreatedAt)
        .ToListAsync();
        }


        public async Task<OwnerVehicleAvailability> GetAvailabilityAtDateAsync(int vehicleId, DateTime date)
        {
            return await _context.OwnerVehicleAvailabilities
                .Where(a => a.VehicleId == vehicleId &&
                           a.EffectiveFrom <= date &&
                           a.EffectiveTo >= date &&
                           a.Status == OwnerVehicleAvailability.AvailabilityStatus.Active)
                .OrderByDescending(a => a.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<OwnerVehicleAvailability> GetLatestAvailabilityAsync(int vehicleId)
        {
            return await _context.OwnerVehicleAvailabilities
                .Where(a => a.VehicleId == vehicleId)
                .OrderByDescending(a => a.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<Vehicle> GetVehicleByIdAsync(int vehicleId)
        {
            return await _context.Vehicles
                .FirstOrDefaultAsync(v => v.VehicleId == vehicleId);
        }
    }


}



