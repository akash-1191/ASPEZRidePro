using System.ComponentModel.DataAnnotations;

namespace EZRide_Project.DTO.Vehile_Owner_DTo
{
    public class OwnerDocumentDTO
    {
        public int DocumentId { get; set; }

        [Required(ErrorMessage = "Document type is required.")]
        public string DocumentType { get; set; } // RCBook, InsurancePaper, AadhaarCard, etc.

        public IFormFile? DocumentFile { get; set; }

        public string? DocumentPath { get; set; } 

        public string? Status { get; set; }

        public string? Reason { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? PublicId { get; set; }



    }
}
