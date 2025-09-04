using EZRide_Project.DTO;
using EZRide_Project.Helpers;
using EZRide_Project.Model.Entities;
using EZRide_Project.Model;
using EZRide_Project.Repositories;

namespace EZRide_Project.Services
{
    public class SecurityDepositService : ISecurityDepositService
    {
        private readonly ISecurityDepositRepository _repository;

        public SecurityDepositService(ISecurityDepositRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponseModel> AddDepositAsync(SecurityDepositDTO dto)
        {
            try
            {
                var deposit = new SecurityDeposit
                {
                    BookingId = dto.BookingId,
                    Amount = dto.Amount,
                    Status = SecurityDeposit.DepositStatus.Pending,
                    CreatedAt = DateTime.Now
                };

                await _repository.AddAsync(deposit);
                var success = await _repository.SaveChangesAsync();

                if (success)
                    return ApiResponseHelper.Success("Security deposit added successfully.", deposit);

                return ApiResponseHelper.Fail("Failed to save security deposit.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.ServerError("An error occurred: " + ex.Message);
            }
        }

        //get perticular security deposit amount

        public async Task<List<SecurityDepositDTO>> GetUserSecurityDepositsAsync(int userId)
        {
            return await _repository.GetSecurityDepositsByUserIdAsync(userId);
        }

    }
}
