using EZRide_Project.Data;
using EZRide_Project.DTO;
using EZRide_Project.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace EZRide_Project.Repositories
{
    public class SecurityDepositRepository : ISecurityDepositRepository
    {
        private readonly ApplicationDbContext _context;

        //add payment
        public SecurityDepositRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SecurityDeposit> AddAsync(SecurityDeposit deposit)
        {
            await _context.SecurityDeposits.AddAsync(deposit);
            return deposit;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }


        //get perticular security deposit amount

        public async Task<List<SecurityDepositDTO>> GetSecurityDepositsByUserIdAsync(int userId)
        {
            return await _context.SecurityDeposits
                .Include(sd => sd.Booking)
                .ThenInclude(b => b.Vehicle)
                .Where(sd => sd.Booking.UserId == userId)

                .Select(sd => new SecurityDepositDTO
                {
                    DepositId = sd.DepositId,
                    BookingId = sd.BookingId,
                    Amount = sd.Amount,
                    Status = sd.Status.ToString(),
                    CreatedAt = sd.CreatedAt,
                    RefundedAt = sd.RefundedAt,
                    VehicleName = sd.Booking.Vehicle.CarName ?? sd.Booking.Vehicle.BikeName,
                    RegistrationNo = sd.Booking.Vehicle.RegistrationNo
                })
                .ToListAsync();
        }


    }
}

