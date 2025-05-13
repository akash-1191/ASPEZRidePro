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
    }
}
