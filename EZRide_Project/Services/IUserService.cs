using EZRide_Project.DTO;
using EZRide_Project.Model;
using EZRide_Project.Model.Entities;

namespace EZRide_Project.Services
{
    public interface IUserService
    {
        ApiResponseModel RegisterUser(AddUserDataDTO dto);


        //Login
        Task<ApiResponseModel> LoginUser(LoginDTO loginDTO,EmailService _emailService);

        //GetuserData
        ApiResponseModel GetUserProfile(int authUserId, int requestedUserId);

    }
}
