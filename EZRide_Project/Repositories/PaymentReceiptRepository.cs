using System;
using EZRide_Project.Data;
using EZRide_Project.DTO;
using Microsoft.EntityFrameworkCore;

namespace EZRide_Project.Repositories
{
    public class PaymentReceiptRepository : IPaymentReceiptRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentReceiptRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentReceiptDto?> GetPaymentReceiptByUserIdAndBookingIdAsync(int userId, int bookingId)
        {
            var user = await _context.Users
                .Where(u => u.UserId == userId)
                .FirstOrDefaultAsync();

            if (user == null)
                return null;

            var bookingData = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Vehicle)
                    .ThenInclude(v => v.VehicleImages)
                .Include(b => b.Payment)
                .Include(b => b.SecurityDeposit)
                .Where(b => b.UserId == userId && b.BookingId == bookingId)  // both filters
                .FirstOrDefaultAsync();

            if (bookingData == null)
                return null;

            var vehicle = bookingData.Vehicle;

            var dto = new PaymentReceiptDto
            {
                VehicleImagePath = vehicle?.VehicleImages?.FirstOrDefault()?.ImagePath ?? "default-image-path",
                FullName = $"{user.Firstname} {(user.Middlename ?? "")} {user.Lastname}".Trim(),
                Email = user.Email,
                PhoneNumber = user.Phone,
                VehicleName = vehicle?.CarName ?? vehicle?.BikeName ?? "N/A",
                VehicleType = vehicle?.Vehicletype.ToString(),
                FuelType = vehicle?.FuelType.ToString(),
                RegistrationNumber = vehicle?.RegistrationNo ?? "N/A",
                StartDateTime = bookingData.StartTime,
                DropDateTime = bookingData.EndTime,
                BookingCreatedAt = bookingData.CreatedAt,
                BookingStatus = bookingData.Status.ToString(),
                PaymentMethod = bookingData.Payment?.PaymentMethod,
                TransactionId = bookingData.Payment?.TransactionId,
                PaymentStatus = bookingData.Payment?.Status,
                TotalPayment = bookingData.Payment?.Amount ?? 0,
                SecurityDepositAmount = bookingData.SecurityDeposit?.Amount ?? 0
            };

            return dto;
        }




        public async Task<PaymentReceiptDto?> GetPaymentReceiptByUserIdAsync(int userId, int bookingId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return null;

            var bookingData = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Vehicle).ThenInclude(v => v.VehicleImages)
                .Include(b => b.Payment)
                .Include(b => b.SecurityDeposit)
                .Where(b => b.UserId == userId && b.BookingId == bookingId)
                .FirstOrDefaultAsync();

            if (bookingData == null) return null;

            var vehicle = bookingData.Vehicle;

            return new PaymentReceiptDto
            {
                VehicleImagePath = vehicle?.VehicleImages?.FirstOrDefault()?.ImagePath ?? "default-image-path",
                FullName = $"{user.Firstname} {(user.Middlename ?? "")} {user.Lastname}".Trim(),
                Email = user.Email,
                PhoneNumber = user.Phone,
                VehicleName = vehicle?.CarName ?? vehicle?.BikeName ?? "N/A",
                VehicleType = vehicle?.Vehicletype.ToString(),
                FuelType = vehicle?.FuelType.ToString(),
                RegistrationNumber = vehicle?.RegistrationNo ?? "N/A",
                StartDateTime = bookingData.StartTime,
                DropDateTime = bookingData.EndTime,
                BookingCreatedAt = bookingData.CreatedAt,
                BookingStatus = bookingData.Status.ToString(),
                PaymentMethod = bookingData.Payment?.PaymentMethod,
                TransactionId = bookingData.Payment?.TransactionId,
                PaymentStatus = bookingData.Payment?.Status,
                TotalPayment = bookingData.Payment?.Amount ?? 0,
                SecurityDepositAmount = bookingData.SecurityDeposit?.Amount ?? 0
            };
        }
    }
}

