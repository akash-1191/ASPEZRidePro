using System.ComponentModel.DataAnnotations;

namespace EZRide_Project.DTO.Vehile_Owner_DTo
{
    public class AddOwnerDocumentDTO
    {
        [Required(ErrorMessage = "Document type is required.")]
        public string DocumentType { get; set; } // RCBook, InsurancePaper, AadhaarCard, etc.

        [Required(ErrorMessage = "Document file is required.")]
        public IFormFile DocumentFile { get; set; } // File upload
    }
}
