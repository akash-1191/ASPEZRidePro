using EZRide_Project.DTO.Vehile_Owner_DTo;
using EZRide_Project.Model;

namespace EZRide_Project.Services
{
    public interface IOwnerDocumentService
    {
        Task<ApiResponseModel> AddOwnerDocumentAsync(int ownerId, AddOwnerDocumentDTO dto);
        Task<List<OwnerDocumentDTO>> GetOwnerDocumentsAsync(int ownerId);
        Task<ApiResponseModel> DeleteOwnerDocumentAsync(int ownerId, int documentId);
        Task<ApiResponseModel> UpdateOwnerDocumentAsync(int ownerId, updateOwnerDocumentDTO dto);
    }
}
