using EZRide_Project.Model.Entities;

namespace EZRide_Project.DTO.Driver_DTO
{
    public class AddDriverDocumentDto
    {
        public int DriverId { get; set; }
        public DriverDocuments.DocumentTypes DocumentType { get; set; }
        public IFormFile DocumentFile { get; set; }
    }
}
