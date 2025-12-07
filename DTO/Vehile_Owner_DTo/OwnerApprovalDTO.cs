namespace EZRide_Project.DTO.Vehile_Owner_DTo
{
    public class OwnerApprovalDTO
    {
        public int OwnerId { get; set; }

        public string Firstname { get; set; }
        public string? Middlename { get; set; }
        public string Lastname { get; set; }

        public string FullName => $"{Firstname} {Middlename} {Lastname}".Replace("  ", " ");

        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public string RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }

        // Owner Document URLs
        public string? RcBook { get; set; }
        public string? InsurancePaper { get; set; }
        public string? AadharCard { get; set; }
    }
}
