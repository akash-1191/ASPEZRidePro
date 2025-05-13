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


        public User GetuserProfile(int id)
        {
            return _context.Users
                .Include(u => u.Role)   
                .FirstOrDefault(u => u.UserId == id);

        }

    }
}
