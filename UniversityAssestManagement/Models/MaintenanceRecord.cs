using UniversityAssetsManagement.Models;

namespace UniversityAssestManagement.Models
{


    public static class MaintenanceStatus
    {
        public const string Pending = "Pending";
        public const string Assigned = "Assigned";
        public const string InProgress = "In Progress";
        public const string Fixed = "Fixed";
        public const string UnderMaintenance = "Under Maintenance";
    }
    public class MaintenanceRecord
    {

      
        public int Id { get; set; }

       public string IssueType { get; set;  } = string.Empty;
        public string IssueDescription { get; set; } = string.Empty;

        public decimal Cost { get; set; }

        public string Status {get; set; } = MaintenanceStatus.Pending;

        public string Severity {  get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.UtcNow;


        //foreign key
        public int AssetId { get; set; }
        public Asset Asset { get; set; } = null!;

        //new fields

        public string? TechnicianName { get; set; }
        public string? ResolutionNotes {  get; set; }
        public DateTime? ResolvedAt { get; set; }


        // Navigation property for the assigned technician
        public int? TechnicianId { get; set; }
        public Technician? Technician { get; set; }
        public DateTime? AssignedAt { get; set; }
        public string? AssignedBy { get; set; }

        
    }
}
