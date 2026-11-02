namespace UniversityAssetsManagement.Models.ViewModels
{
    public class AssetsDetailsViewModel
    {

        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Type { get; set; }= string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string Description {  get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public string CollegeName { get; set; } = string.Empty;

        //maintenance history
        public List<CollegeMaintenanceRow> MaintenanceRecords { get; set; } = new();
        public int TotalMaintenances { get; set; }
        public decimal TotalRepairCost { get; set; }

    }

    public class CollegeMaintenanceRow
    {
        public string IssueDescription { get; set; }= string.Empty;
        public decimal Cost { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime Date {  get; set; }
    }
}
