using EZRide_Project.DTO;
using EZRide_Project.Repositories;

namespace EZRide_Project.Services
{
    public class BookingSummaryService : IBookingSummaryService
    {
        private readonly IBookingSummaryRepository _bookingRepo;

        public BookingSummaryService(IBookingSummaryRepository bookingRepo)
        {
            _bookingRepo = bookingRepo;
        }

        public BookingSummaryDTO GetTotalBookingsByUserId(int userId)
        {
            var total = _bookingRepo.GetTotalBookingsByUserId(userId);
            return new BookingSummaryDTO { TotalBookings = total };
        }


        public async Task<VehicleBookingCountDTO> GetBookedVehicleTypeCountAsync(int userId)
        {

            return await _bookingRepo.GetBookedVehicleTypeCountAsync(userId);
        }


        //total vehicle avalible for booking
        public int GetAvailableVehicleCount()
        {
            return _bookingRepo.GetAvailableVehicleCount();
        }


        //get the payment status pending
        public async Task<int> GetPendingPaymentCountAsync(int userId)
        {
            return await _bookingRepo.GetPendingPaymentCountAsync(userId);
        }



        //get the amount of of refaund with date and time 
        public Task<RefundInfoDto?> GetLatestRefundAsync(int userId)
        {
            return _bookingRepo.GetLatestRefundAsync(userId);
        }
    }

}
