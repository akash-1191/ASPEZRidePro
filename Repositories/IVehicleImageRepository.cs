using EZRide_Project.Model.Entities;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace EZRide_Project.Repositories
{
    public interface IVehicleImageRepository
    {

        //Add image 
        Task AddVehicleImageAsync(VehicleImage image);
        Task<bool> SaveChangesAsync();

        //update Image
        Task<VehicleImage?> GetVehicleImageByIdAsync(int vehicleImageId);
        void UpdateVehicleImage(VehicleImage image);

        //Delete Image
       
        void DeleteVehicleImage(VehicleImage image);
      //Get Aall Image 
        Task<List<VehicleImage>> GetImagesByVehicleIdAsync(int vehicleId);
    }
}

