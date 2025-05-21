using EZRide_Project.Data;
using EZRide_Project.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace EZRide_Project.Repositories
{
    public class VehicleImageRepository : IVehicleImageRepository
    {
        private readonly ApplicationDbContext _context;

        public VehicleImageRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        //add image
        public async Task AddVehicleImageAsync(VehicleImage image)
        {
            await _context.VehicleImages.AddAsync(image);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        //update image
        public async Task<VehicleImage?> GetVehicleImageByIdAsync(int vehicleImageId)
        {
            return await _context.VehicleImages.FirstOrDefaultAsync(i => i.VehicleImageId == vehicleImageId);
        }

        public void UpdateVehicleImage(VehicleImage image)
        {
            _context.VehicleImages.Update(image);
        }


        //delete image
        public void DeleteVehicleImage(VehicleImage image)
        {
            _context.VehicleImages.Remove(image);
        }
        //get vehicle image
        public async Task<List<VehicleImage>> GetImagesByVehicleIdAsync(int vehicleId)
        {
            return await _context.VehicleImages
                         .Where(i => i.VehicleId == vehicleId)
                         .ToListAsync();
        }
    }
}
