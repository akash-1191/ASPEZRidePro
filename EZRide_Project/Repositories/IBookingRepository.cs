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

        //get all data of the booking table 
        List<Booking> GetBookingsByUserId(int userId);
    }
}
