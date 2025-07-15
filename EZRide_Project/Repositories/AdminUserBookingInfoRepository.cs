using System;
using EZRide_Project.Data;
using EZRide_Project.DTO;
using EZRide_Project.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace EZRide_Project.Repositories
{
    public class AdminUserBookingInfoRepository : IAdminUserBookingInfoRepository
    {
        private readonly ApplicationDbContext _context;

        public AdminUserBookingInfoRepository(ApplicationDbContext context)
        {
            _context = context;
        }   

        public async Task<List<AdminUserBookingInfoDto>> GetUserBookingInfoAsync()
        {
            var query = _context.Bookings
                .Include(b => b.User)
                    .ThenInclude(u => u.Role)
                .Include(b => b.Vehicle)
                    .ThenInclude(v => v.VehicleImages)
                .Include(b => b.Payment)
                .Include(b => b.SecurityDeposit)
                 .Where(b =>
            (b.Status == Booking.BookingStatus.Confirmed || b.Status == Booking.BookingStatus.InProgress) &&
            b.Payment.Status == "Success")
                .Select(booking => new AdminUserBookingInfoDto
                {
                    // Booking info
                    BookingId = booking.BookingId,
                    StartTime = booking.StartTime,
                    EndTime = booking.EndTime,
                    TotalAmount = booking.TotalAmount,
                    BookingStatus = booking.Status.ToString(),

                    BookingType = booking.BookingType,
                    TotalDays = booking.TotalDays,
                    TotalHours = booking.TotalHours,
                    PerKelomeater = booking.PerKelomeater,
                    BookingCreatedAt = booking.CreatedAt,

                    // Payment info
                    PaymentStatus = booking.Payment.Status,
                    PaymentAmount = booking.Payment.Amount,
                    PaymentMethod = booking.Payment.PaymentMethod,
                    TransactionId = booking.Payment.TransactionId,
                    OrderId = booking.Payment.OrderId,
                    PaymentCreatedAt = booking.Payment.CreatedAt,

                    // Security Deposit info
                    SecurityDepositStatus = booking.SecurityDeposit.Status.ToString(),
                    SecurityDepositAmount = booking.SecurityDeposit.Amount,
                    RefundedAt = booking.SecurityDeposit.RefundedAt,
                    SecurityDepositCreatedAt = booking.SecurityDeposit.CreatedAt,

                    // User info
                    UserId = booking.User.UserId,
                    FirstName = booking.User.Firstname,
                    MiddleName = booking.User.Middlename,
                    LastName = booking.User.Lastname,
                    Email = booking.User.Email,
                    Phone = booking.User.Phone,
                    Address = booking.User.Address,
                    City = booking.User.City,
                    State = booking.User.State,
                    Age = booking.User.Age,
                    Gender = booking.User.Gender,
                    UserImage = booking.User.Image,
                    RoleName = booking.User.Role.RoleName.ToString(),
                    UserCreatedAt=booking.User.CreatedAt,


                    // Vehicle info
                    VehicleId = booking.Vehicle.VehicleId,
                    VehicleType = booking.Vehicle.Vehicletype.ToString(),
                    RegistrationNo = booking.Vehicle.RegistrationNo,
                    FuelType = booking.Vehicle.FuelType.ToString(),
                    SeatingCapacity = booking.Vehicle.SeatingCapacity,
                    Mileage = booking.Vehicle.Mileage,
                    Color = booking.Vehicle.Color,
                    CarName = booking.Vehicle.CarName,
                    BikeName = booking.Vehicle.BikeName,
                    Availability = booking.Vehicle.Availability.ToString(),
                    InsuranceStatus = booking.Vehicle.InsuranceStatus.ToString(),
                    RcStatus = booking.Vehicle.RcStatus.ToString(),
                    AcAvailability = booking.Vehicle.AcAvailability.HasValue ? booking.Vehicle.AcAvailability.ToString() : null,
                    FuelTankCapacity = booking.Vehicle.FuelTankCapacity,
                    YearOfManufacture = booking.Vehicle.YearOfManufacture,
                    EngineCapacity = booking.Vehicle.EngineCapacity,
                    VehicleSecurityDepositAmount = booking.Vehicle.SecurityDepositAmount,
                    VehicleCreatedAt = booking.Vehicle.CreatedAt,


                    // Vehicle image - first image or null
                    VehicleImage = booking.Vehicle.VehicleImages
                        .OrderByDescending(img => img.VehicleImageId) // assuming VehicleImage has Id or CreatedAt
                        .Select(img => img.ImagePath)
                        .FirstOrDefault(),


            VehicleImages = booking.Vehicle.VehicleImages
                       .OrderBy(img => img.VehicleImageId)
                       .Select(img => img.ImagePath)
                       .ToList()
                });
            return await query.ToListAsync();
        }

    }
}
