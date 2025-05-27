using EZRide_Project.Data;
using EZRide_Project.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace EZRide_Project.Repositories
{
      
        public class PaymentRepository : IPaymentRepository
        {
            private readonly ApplicationDbContext _context;

            public PaymentRepository(ApplicationDbContext context)
            {
                _context = context;
            }

        public async Task<bool> SavePaymentAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            return await _context.SaveChangesAsync() > 0;
        }

    }
    
}
