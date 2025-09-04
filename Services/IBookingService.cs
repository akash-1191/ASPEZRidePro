using EZRide_Project.DTO;
using EZRide_Project.Model;
using EZRide_Project.Model.Entities;

namespace EZRide_Project.Services
{
    public interface IBookingService
    {
        ApiResponseModel AddBooking(BookingDTO dto);


        // Cancel booking method
        ApiResponseModel CancelBooking(int bookingId, int userId);


        //get all full user data 
        Task<List<BookingDetailDTO>> GetUserBookingsAsync(int userId);

        //filter the data
        Task<List<BookingDetailDTO>> FilterUserBookingsAsync(int userId, BookingFilterDTO filter);

        //check the booking is avalible or not live
        Task<List<DateAvailabilityDTO>> GetAvailabilityAsync(int vehicleId, DateTime startDateTime, DateTime endDateTime);

    }
}
