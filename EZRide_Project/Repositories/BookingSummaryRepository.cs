using EZRide_Project.Data;
using EZRide_Project.DTO;
using EZRide_Project.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace EZRide_Project.Repositories
{
    public class BookingSummaryRepository : IBookingSummaryRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingSummaryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public int GetTotalBookingsByUserId(int userId)
        {
            return _context.Bookings.Count(b => b.UserId == userId);
        }



        //get how many bike or cars bookied by user
        public async Task<VehicleBookingCountDTO> GetBookedVehicleTypeCountAsync()
        {
            var bikeCount = await _context.Bookings
                .Include(b => b.Vehicle)
                .Where(b => b.Vehicle.Vehicletype == Vehicle.VehicleType.Bike)
                .CountAsync();

            var carCount = await _context.Bookings
                .Include(b => b.Vehicle)
                .Where(b => b.Vehicle.Vehicletype == Vehicle.VehicleType.Car)
                .CountAsync();

            return new VehicleBookingCountDTO
            {
                BikeCount = bikeCount,
                CarCount = carCount
            };
        }

        //total vehicle avalible for booking

        public int GetAvailableVehicleCount()
        {
            return _context.Vehicles.Count(v => v.Availability == Vehicle.AvailabilityStatus.Available);
        }

        //get the payment status pending
        public async Task<int> GetPendingPaymentCountAsync(int userId)
        {
            var today = DateTime.Now.Date;

            var pendingCount = await _context.Bookings
                .Where(b => b.UserId == userId)
                .Where(b => b.EndTime >= today) // only consider current/future bookings
                .Where(b => !_context.Payments
                    .Any(p => p.BookingId == b.BookingId && p.Status.ToLower() == "success"))
                .CountAsync();

            return pendingCount;
        }


        //get the amount of of refaund with date and time 

        public async Task<RefundInfoDto?> GetLatestRefundAsync(int userId)
        {
            var latestRefund = await _context.SecurityDeposits
                .Where(sd => sd.Status == SecurityDeposit.DepositStatus.Refunded)
                .Where(sd => sd.Booking.UserId == userId)
                .OrderByDescending(sd => sd.RefundedAt) 
                .Select(sd => new RefundInfoDto
                {
                    Amount = sd.Amount,
                    RefundedAt = sd.RefundedAt ?? DateTime.MinValue
                })
                .FirstOrDefaultAsync();

            return latestRefund;
        }

    }
}
