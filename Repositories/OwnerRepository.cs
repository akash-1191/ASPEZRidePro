using System;
using EZRide_Project.Data;
using EZRide_Project.DTO.Vehile_Owner_DTo;
using EZRide_Project.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace EZRide_Project.Repositories
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly ApplicationDbContext _context;

        public OwnerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<OwnerApprovalDTO>> GetPendingOwnersAsync()
        {

            return await _context.Users
       .Include(u => u.Role)
       .Include(u => u.OwnerDocuments)
       .Where(u =>
           u.Role.RoleName == Role.Rolename.OwnerVehicle)
       .Select(u => new OwnerApprovalDTO
       {
           OwnerId = u.UserId,
           Firstname = u.Firstname,
           Middlename = u.Middlename,
           Lastname = u.Lastname,
           Email = u.Email,
           Phone = u.Phone,
           Address = u.Address,
           Status = u.Status.ToString(),
           RejectionReason = u.RejectionReason ?? "",
           CreatedAt = u.CreatedAt,

           // RC Book
           RcBook = u.OwnerDocuments
                        .Where(d => d.DocumentType == OwnerDocument.documentType.RCBook)
                        .Select(d => d.DocumentPath)
                        .FirstOrDefault(),

           // Insurance
           InsurancePaper = u.OwnerDocuments
                        .Where(d => d.DocumentType == OwnerDocument.documentType.InsurancePaper)
                        .Select(d => d.DocumentPath)
                        .FirstOrDefault(),

           // Aadhar
           AadharCard = u.OwnerDocuments
                        .Where(d => d.DocumentType == OwnerDocument.documentType.AadharCard)
                        .Select(d => d.DocumentPath)
                        .FirstOrDefault(),

       })
                .ToListAsync();
        }

        public User GetUserById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.UserId == id);
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();

        }

        //Get all vehicles uploaded by owners
        //public async Task<List<Vehicle>> GetAllOwnerVehiclesAsync(int ownerId)
        //{
        //    return await _context.Vehicles
        //        .Include(v => v.User)
        //            .ThenInclude(u => u.Role)
        //        .Where(v => v.User != null
        //                    && v.User.Role != null
        //                    && v.User.Role.RoleName == Role.Rolename.OwnerVehicle
        //                    && v.UserId == ownerId)
        //        .ToListAsync();
        //}
        public async Task<List<Vehicle>> GetAllOwnerVehiclesAsync(int ownerId)
        {
            return await _context.Vehicles
                .Include(v => v.User)
                    .ThenInclude(u => u.Role)
                .Include(v => v.OwnerVehicleAvailabilities)
                .Where(v => v.User != null
                            && v.User.Role != null
                            && v.User.Role.RoleName == Role.Rolename.OwnerVehicle
                            && v.UserId == ownerId)
                .ToListAsync();
        }


        //get aal aproval owner 
        public async Task<List<User>> GetAllActiveOwnersAsync()
        {
            return await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Role != null
                            && u.Role.RoleName == Role.Rolename.OwnerVehicle
                            && u.Status == User.UserStatus.Active)
                .ToListAsync();
        }


        //add or update the security amount
        public async Task<Vehicle> GetVehicleByIdAsync(int vehicleId)
        {
            return await _context.Vehicles.FirstOrDefaultAsync(v => v.VehicleId == vehicleId);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }


        public async Task UpdateVehicleAsync(Vehicle vehicle)
        {
            _context.Vehicles.Update(vehicle);
            await _context.SaveChangesAsync();
        }


        public async Task<OwnerVehicleAvailability> GetAvailabilityByIdAsync(int availabilityId)
        {
            return await _context.OwnerVehicleAvailabilities
                .FirstOrDefaultAsync(a => a.AvailabilityId == availabilityId);
        }

        public async Task UpdateAvailabilityAsync(OwnerVehicleAvailability availability)
        {
            _context.OwnerVehicleAvailabilities.Update(availability);
            await _context.SaveChangesAsync();
        }
    }
}

