using EZRide_Project.DTO;
using EZRide_Project.Model.Entities;

namespace EZRide_Project.Repositories
{
    public interface ISecurityDepositRepository
    {
        //add payment

        Task<SecurityDeposit> AddAsync(SecurityDeposit deposit);
        Task<bool> SaveChangesAsync();

        //get perticular security deposit amount
        Task<List<SecurityDepositDTO>> GetSecurityDepositsByUserIdAsync(int userId);
    }
}
