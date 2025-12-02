using System.ComponentModel.DataAnnotations;

namespace EZRide_Project.DTO.Vehile_Owner_DTo
{
    public class updateOwnerDocumentDTO
    {
        public int DocumentId { get; set; }

        [Required(ErrorMessage = "Document type is required.")]
        public string DocumentType { get; set; } // RCBook, InsurancePaper, AadhaarCard, etc.

        [Required(ErrorMessage = "Document file is required.")]
        public IFormFile DocumentFile { get; set; } 
    }
}
