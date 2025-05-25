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

        // get aal bookin data 
        ApiResponseModel GetBookingsByUserId(int userId);
    }
}
