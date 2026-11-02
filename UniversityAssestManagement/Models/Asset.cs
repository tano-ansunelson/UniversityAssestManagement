using System.Security.Cryptography.X509Certificates;

namespace UniversityAssestManagement.Models
{
    public static class AssetType
    {
        public const string Unique = "Unique";
        public const string Bulk = "Bulk";

    }

    public static class AssetStatus
    {
        public const string Active = "Active";
        public const string Damaged = "Damaged";
        public const string UnderMaintenance = "Under Maintenance";
        public const string Malfunction = "Malfunction";
        public const string Missing = "Missing";
        public const string MaintenanceNeeded = " Maintenance Needed";

    }





    public class Asset
    {

        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Type { get; set; } = AssetType.Unique;

        public decimal Value { get; set; }

        public string Status { get; set; } = AssetStatus.Active;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? Quantity { get; set; }
        public string? Description { get; set; }
        public int CollegeId { get; set; }
        public int LocationId { get; set; }

        public College College { get; set; } = null!;

        public Location Location { get; set; } = null!;

        public string? UploadedBy { get; set; }

        public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; }  = new List<MaintenanceRecord>();


    }
}
