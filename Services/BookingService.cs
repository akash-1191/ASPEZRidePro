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
            if (_bookingRepository.IsBookingOverlapping(dto.VehicleId, dto.StartTime, dto.EndTime))
            {
                return ApiResponseHelper.Fail("Same Date And Time Vehicle is already have a booking in the selected time range.", 409);
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
                Status = Booking.BookingStatus.Confirmed,
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

            var timeDifference = booking.StartTime - DateTime.Now;
            if (timeDifference.TotalHours < 2)
            {
                return ApiResponseHelper.Fail("Cannot cancel the booking. Cancellation is allowed only 2 hours before trip start time.", 400);
            }

            _bookingRepository.CancelBooking(bookingId, userId);
            return ApiResponseHelper.Success("Booking cancelled successfully.");
        }





        //get all full data of the useer

        public async Task<List<BookingDetailDTO>> GetUserBookingsAsync(int userId)
        {
            return await _bookingRepository.GetUserBookingsAsync(userId);
        }

        
        public async Task<List<BookingDetailDTO>> FilterUserBookingsAsync(int userId, BookingFilterDTO filter)
        {
            return await _bookingRepository.FilterUserBookingsAsync(userId, filter);
        }


        //check the booking is avalible or not live
        public async Task<List<DateAvailabilityDTO>> GetAvailabilityAsync(int vehicleId, DateTime startDate, DateTime endDate)
        {
            return await _bookingRepository.GetAvailabilityAsync(vehicleId, startDate, endDate);
        }
    }
}
