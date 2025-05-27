using EZRide_Project.Data;
using EZRide_Project.DTO;
using EZRide_Project.Model.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;

namespace EZRide_Project.Repositories
{

    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddBooking(Booking booking)
        {
            _context.Bookings.Add(booking);
            _context.SaveChanges();
        }

        public bool IsBookingOverlapping(int userId, DateTime startTime, DateTime endTime)
        {
            return _context.Bookings.Any(b =>
                b.UserId == userId &&
                b.Status != Booking.BookingStatus.Cancelled &&
                (
                    (startTime >= b.StartTime && startTime < b.EndTime) ||
                    (endTime > b.StartTime && endTime <= b.EndTime) ||
                    (startTime <= b.StartTime && endTime >= b.EndTime)
                )
            );
        }
        public void CancelBooking(int bookingId, int userId)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == bookingId && b.UserId == userId);
            if (booking != null)
            {
                booking.Status = Booking.BookingStatus.Cancelled;
                _context.SaveChanges();
            }
        }
        public Booking? GetBookingByIdAndUser(int bookingId, int userId)
        {
            return _context.Bookings.FirstOrDefault(b => b.BookingId == bookingId && b.UserId == userId);
        }



        //get all full data
        public async Task<List<BookingDetailDTO>> GetUserBookingsAsync(int userId)
        {
            var bookings = await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Vehicle)
                .Include(b => b.Payment)
                .ToListAsync();

            var result = new List<BookingDetailDTO>();

            foreach (var booking in bookings)
            {
                var vehicleImage = await _context.VehicleImages
                    .Where(v => v.VehicleId == booking.VehicleId)
                    .Select(v => v.ImagePath)
                    .FirstOrDefaultAsync();

                result.Add(new BookingDetailDTO
                {
                    VehicleImage = vehicleImage,
                    VehicleType = booking.Vehicle.Vehicletype.ToString(),
                    VehicleName = booking.Vehicle.Vehicletype == Vehicle.VehicleType.Bike
                                  ? booking.Vehicle.BikeName
                                  : booking.Vehicle.CarName,
                    RegistrationNo = booking.Vehicle.RegistrationNo,
                    FuelType = booking.Vehicle.FuelType.ToString(),
                    BookingDateTime = booking.StartTime,
                    DropOffDateTime = booking.EndTime,
                    BookingType = booking.BookingType == "perDay" ? $"{booking.TotalDays} Days"
                                    : booking.BookingType == "perHour" ? $"{booking.TotalHours} Hours"
                                    : booking.BookingType == "perKm" ? $"{booking.PerKelomeater} KM" : "-",
                    TotalAmount = booking.TotalAmount,
                    PaymentStatus = booking.Payment?.Status ?? "N/A",
                    PaymentMode = booking.Payment?.PaymentMethod ?? "N/A",
                    TransactionId = booking.Payment?.TransactionId ?? "N/A",
                    BookingStatus = booking.Status.ToString(),
                    CreatedAt=booking.CreatedAt
                });
            }

            return result;
        }


        //filter data of the booking table

        public async Task<List<BookingDetailDTO>> FilterUserBookingsAsync(int userId, BookingFilterDTO filter)
        {
            var query = _context.Bookings
                .Include(b => b.Vehicle)
                .Include(b => b.Payment)
                .Where(b => b.UserId == userId)
                .AsQueryable();

            // Booking Status
            if (!string.IsNullOrEmpty(filter.BookingStatus) &&
                Enum.TryParse<Booking.BookingStatus>(filter.BookingStatus, true, out var bookingStatusEnum))
            {
                query = query.Where(b => b.Status == bookingStatusEnum);
            }

            // Payment Status
            if (!string.IsNullOrEmpty(filter.PaymentStatus))
            {
                query = query.Where(b => b.Payment != null &&
                                         b.Payment.Status.ToLower() == filter.PaymentStatus.ToLower());
            }

            // Filter by TotalDays
            if (filter.MinDays.HasValue)
            {
                query = query.Where(b => b.TotalDays.HasValue && b.TotalDays.Value >= filter.MinDays.Value);
            }

            // Filter by TotalHours
            if (filter.MinHours.HasValue)
            {
                query = query.Where(b => b.TotalHours.HasValue && b.TotalHours.Value >= filter.MinHours.Value);
            }

            // Filter by PerKelomeater
            if (filter.MinKilometers.HasValue)
            {
                query = query.Where(b => b.PerKelomeater.HasValue && b.PerKelomeater.Value >= filter.MinKilometers.Value);
            }

            // Sorting
            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                if (filter.SortBy.ToLower() == "latest")
                    query = query.OrderByDescending(b => b.CreatedAt);
                else if (filter.SortBy.ToLower() == "old")
                    query = query.OrderBy(b => b.CreatedAt);
            }

            var bookings = await query.ToListAsync();

            var result = new List<BookingDetailDTO>();
            foreach (var booking in bookings)
            {
                var vehicleImage = await _context.VehicleImages
                    .Where(v => v.VehicleId == booking.VehicleId)
                    .Select(v => v.ImagePath)
                    .FirstOrDefaultAsync();

                result.Add(new BookingDetailDTO
                {
                    VehicleImage = vehicleImage,
                    VehicleType = booking.Vehicle.Vehicletype.ToString(),
                    VehicleName = booking.Vehicle.Vehicletype == Vehicle.VehicleType.Bike
                                  ? booking.Vehicle.BikeName
                                  : booking.Vehicle.CarName,
                    RegistrationNo = booking.Vehicle.RegistrationNo,
                    FuelType = booking.Vehicle.FuelType.ToString(),
                    BookingDateTime = booking.StartTime,
                    DropOffDateTime = booking.EndTime,
                    BookingType = booking.BookingType == "perDay" ? $"{booking.TotalDays} Days"
                                  : booking.BookingType == "perHour" ? $"{booking.TotalHours} Hours"
                                  : booking.BookingType == "perKm" ? $"{booking.PerKelomeater} KM" : "-",
                    TotalAmount = booking.TotalAmount,
                    PaymentStatus = booking.Payment?.Status ?? "N/A",
                    PaymentMode = booking.Payment?.PaymentMethod ?? "N/A",
                    TransactionId = booking.Payment?.TransactionId ?? "N/A",
                    BookingStatus = booking.Status.ToString(),
                    CreatedAt = booking.CreatedAt
                });
            }

            return result;
        }


        //public async Task<IEnumerable<booking>> GetBookingsByUser(int? userId)
        //{
        //    IQueryable<Booking> query = _context.Bookings;

        //    if (userId.HasValue)
        //        query = query.Where(b => b.UserId == userId.Value);

        //    var bookings = await query
        //        .Select(b => new BookingDTO
        //        {
        //            BookingId = b.BookingId,
        //            UserId = b.UserId,
        //            VehicleId = b.VehicleId,
        //            StartTime = b.StartTime,
        //            EndTime = b.EndTime,
        //            TotalDistance = b.TotalDistance,
        //            TotalAmount = b.TotalAmount,
        //            BookingType = b.BookingType,
        //            TotalDays = b.TotalDays,
        //            TotalHours = b.TotalHours,
        //            PerKelomeater = b.PerKelomeater,
        //            Status = b.Status.ToString(),

        //            CreatedAt = b.CreatedAt
        //        })
        //        .ToListAsync();

        //    return bookings;
        //}

    }

}
