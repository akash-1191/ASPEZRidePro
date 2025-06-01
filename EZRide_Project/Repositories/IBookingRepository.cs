using EZRide_Project.DTO;
using EZRide_Project.Model.Entities;

namespace EZRide_Project.Repositories
{
    public interface IBookingRepository
    {
        void AddBooking(Booking booking);
        bool IsBookingOverlapping(int userId, DateTime startTime, DateTime endTime);

        // Add this method for update statuse for cancel booking 
        Booking? GetBookingByIdAndUser(int bookingId, int userId);
        void CancelBooking(int bookingId, int userId);


        //get full all data 
        Task<List<BookingDetailDTO>> GetUserBookingsAsync(int userId);

        //filkter data of the booking table
        Task<List<BookingDetailDTO>> FilterUserBookingsAsync(int userId, BookingFilterDTO filter);


        //check the booking is avalible or not live
        Task<List<DateAvailabilityDTO>> GetAvailabilityAsync(int vehicleId, DateTime startDate, DateTime endDate);
    }
}
