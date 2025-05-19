using EZRide_Project.Data;
using EZRide_Project.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace EZRide_Project.Repositories
{
    public class UserRepository:IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool IsEmailExists(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        //Login 
        public User GetUserByEmail(string email)
        {

            return _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email == email);
          
        }

        // user profile data
        public User GetuserProfile(int id)
        {
            return _context.Users
                .Include(u => u.Role)   
                .FirstOrDefault(u => u.UserId == id);
        }


        //update user data
        public User GetuserById(int userId)
        {
           return _context.Users.FirstOrDefault(u => u.UserId == userId);
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        //update user profile image
        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
