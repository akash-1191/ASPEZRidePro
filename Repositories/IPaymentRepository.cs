using EZRide_Project.Model.Entities;

namespace EZRide_Project.Repositories
{
    public interface IPaymentRepository
    {
        Task<bool> SavePaymentAsync(Payment payment);
    }
} 
