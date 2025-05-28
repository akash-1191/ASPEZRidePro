using EZRide_Project.DTO;
using EZRide_Project.Model;

namespace EZRide_Project.Services
{
    //add payment
    public interface ISecurityDepositService
    {
        Task<ApiResponseModel> AddDepositAsync(SecurityDepositDTO dto);
        //get perticular security deposit amount
        Task<List<SecurityDepositDTO>> GetUserSecurityDepositsAsync(int userId);
    }
}
