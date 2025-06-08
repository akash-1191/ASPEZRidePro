using EZRide_Project.Data;
using EZRide_Project.DTO;
using Microsoft.EntityFrameworkCore;

namespace EZRide_Project.Repositories
{
   public class UserBookingDetailsRepository : IUserBookingDetailsRepository
    {
        private readonly ApplicationDbContext _context;

        public UserBookingDetailsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserBookingDetailsDto>> GetAllUsersBookingDetailsAsync()
        {
            var users = await _context.Users
                .Include(u => u.Vehicles)
                .Include(u => u.Bookings)
                    .ThenInclude(b => b.Payment)
                .Include(u => u.Bookings)
                    .ThenInclude(b => b.SecurityDeposit)
                .ToListAsync();

            var dtos = users.Select(user => new UserBookingDetailsDto
            {
                UserId = user.UserId,
                FullName = $"{user.Firstname} {user.Lastname}",
                Email = user.Email,
                Phone = user.Phone,
                Vehicles = user.Vehicles.Select(v => new VehicleDto
                {
                    VehicleId = v.VehicleId,
                    RegistrationNo = v.RegistrationNo,
                    VehicleType = v.Vehicletype.ToString(),
                    Availability = v.Availability.ToString(),
                    Color = v.Color,
                    YearOfManufacture = v.YearOfManufacture
                }).ToList(),
                Bookings = user.Bookings.Select(b => new BookingDto
                {
                    BookingId = b.BookingId,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    TotalAmount = b.TotalAmount,
                    Status = b.Status.ToString(),
                    Payment = b.Payment == null ? null : new PaymentDetailDto
                    {
                        Id = b.Payment.Id,
                        Amount = b.Payment.Amount,
                        TransactionId = b.Payment.TransactionId,
                        PaymentMethod = b.Payment.PaymentMethod,
                        Status = b.Payment.Status
                    },
                    SecurityDeposit = b.SecurityDeposit == null ? null : new SecurityDepositDto
                    {
                        DepositId = b.SecurityDeposit.DepositId,
                        Amount = b.SecurityDeposit.Amount,
                        Status = b.SecurityDeposit.Status.ToString(),
                        RefundedAt = b.SecurityDeposit.RefundedAt
                    }
                }).ToList()
            }).ToList();

            return dtos;
        }
    }
}
