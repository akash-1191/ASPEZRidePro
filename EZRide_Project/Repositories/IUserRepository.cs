using EZRide_Project.Model.Entities;

namespace EZRide_Project.Repositories
{
    public interface IUserRepository
    {
        bool IsEmailExists(string email);
        void AddUser(User user);

        //Login
        User GetUserByEmail(string email);

//GetUserdata
        User GetuserProfile(int id);

//update profile data
        User GetuserById(int userId);
        void UpdateUser(User user);

//update user profile image
        Task<User> GetUserByIdAsync(int userId);
        Task UpdateUserAsync(User user);
    }
}
