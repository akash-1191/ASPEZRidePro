using EZRide_Project.DTO;
using EZRide_Project.Helpers;
using EZRide_Project.Model;
using EZRide_Project.Model.Entities;
using EZRide_Project.Repositories;

namespace EZRide_Project.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public ApiResponseModel AddBooking(BookingDTO dto)
        {
            // Validation
            if (_bookingRepository.IsBookingOverlapping(dto.UserId, dto.StartTime, dto.EndTime))
            {
                return ApiResponseHelper.Fail("You already have a booking in the selected time range.", 409);
            }

            var booking = new Booking
            {
                UserId = dto.UserId,
                VehicleId = dto.VehicleId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                TotalDistance = dto.TotalDistance,
                TotalAmount = dto.TotalAmount,
                BookingType = dto.BookingType,
                TotalDays = dto.TotalDays,
                TotalHours = dto.TotalHours,
                PerKelomeater = dto.PerKelomeater,
                Status = Booking.BookingStatus.Pending,
                CreatedAt = DateTime.Now
            };

            _bookingRepository.AddBooking(booking);

            dto.BookingId = booking.BookingId;
            dto.Status = booking.Status.ToString();
            dto.CreatedAt = booking.CreatedAt;

            return ApiResponseHelper.Success("Booking created successfully.", dto);
        }

        public ApiResponseModel CancelBooking(int bookingId, int userId)
        {
            // Check if booking exists and belongs to the user
            var booking = _bookingRepository.GetBookingByIdAndUser(bookingId, userId);
            if (booking == null)
            {
                return ApiResponseHelper.Fail("Booking not found.", 404);
            }

            if (booking.Status == Booking.BookingStatus.Cancelled)
            {
                return ApiResponseHelper.Fail("Booking is already cancelled.", 400);
            }

            _bookingRepository.CancelBooking(bookingId, userId);
            return ApiResponseHelper.Success("Booking cancelled successfully.");
        }

        //get all data of the booking table
        public ApiResponseModel GetBookingsByUserId(int userId)
        {
            var bookings = _bookingRepository.GetBookingsByUserId(userId);

            if (bookings == null || bookings.Count == 0)
            {
                return ApiResponseHelper.Fail("No bookings found for this user.", 404);
            }

            var dtoList = bookings.Select(b => new BookingDTO
            {
                BookingId = b.BookingId,
                UserId = b.UserId,
                VehicleId = b.VehicleId,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                TotalDistance = b.TotalDistance,
                TotalAmount = b.TotalAmount,
                BookingType = b.BookingType,
                TotalDays = b.TotalDays,
                TotalHours = b.TotalHours,
                PerKelomeater=b.PerKelomeater,
                Status = b.Status.ToString(),
                CreatedAt = b.CreatedAt
            }).ToList();

            return ApiResponseHelper.Success("Bookings fetched successfully.", dtoList);
        }
    }
}
