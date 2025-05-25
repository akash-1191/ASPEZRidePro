using EZRide_Project.Data;
using EZRide_Project.Model.Entities;
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

        //get all data of the booking
        public List<Booking> GetBookingsByUserId(int userId)
        {
            return _context.Bookings
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToList();
        }
    }

}
