using EZRide_Project.Data;
using EZRide_Project.DTO;
using Microsoft.EntityFrameworkCore;

namespace EZRide_Project.Repositories
{
    public class PaymentDetailsRepository : IPaymentDetailsRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentDetailsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserBookingPaymentDetailsDto>> GetUserBookingPaymentsAsync(int userId)
        {
            return await _context.Bookings
                
                .Include(b => b.Vehicle)
                .Include(b => b.Payment)
                .Include(b => b.SecurityDeposit)
                .Where(b => b.UserId == userId)
                .Select(b => new UserBookingPaymentDetailsDto
                {   
                    Bookingid=b.BookingId,
                    VehicleName = b.Vehicle.CarName ?? b.Vehicle.BikeName,
                    VehicleType = b.Vehicle.Vehicletype.ToString(),
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    TotalAmount = b.TotalAmount,
                    SecurityDepositAmount = b.Vehicle.SecurityDepositAmount != null ? b.Vehicle.SecurityDepositAmount : 0,
                    PaymentStatus = b.Payment != null ? b.Payment.Status : "Not Paid"
                })
                .ToListAsync();
        }
    }
}
