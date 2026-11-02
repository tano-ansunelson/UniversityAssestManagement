using System.ComponentModel.DataAnnotations;
using UniversityAssestManagement.Models;
using UniversityAssestManagement.Models.ViewModels;

namespace UniversityAssetsManagement.Models.ViewModels
{
    public class UploadAssetViewModel
    {
        [Required(ErrorMessage ="Asset name is required.")]
        [Display(Name ="Asset Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Type is required. ")]
        public string Type { get; set; } = AssetType.Unique;

        [Required(ErrorMessage ="Please select a location")]
        [Display(Name ="Location")]
        public int? LocationId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage ="Quantity must be at least 1.")]

        public int? Quantity { get; set; }
        public string? NewLocation { get; set; }
        public string LocationOption { get; set; } = "select";

        [Range(0, double.MaxValue, ErrorMessage ="Value must be positive")]
        public decimal Value { get; set; }

        public string? Description { get; set; }
        public List<IFormFile>? Files { get; set; }

        public List<LocationDropdownItem> Locations { get; set; } = new();





    }


    public class LocationDropdownItem
    {
        public int Id { get; set; }
        public string Name {  set; get; } = string.Empty;
    }


    public class ReportIssueViewModel
    {
        //pre-filled from asset - not entered by user
        public int AssetId { get; set; }

        [Required(ErrorMessage = "Asset name is requirred.")]
        [Display(Name = "Asset Name")]
        public string Name { get; set; } = string.Empty;


        [Required(ErrorMessage = "Location is required. ")]
        public string LocationName { get; set; } = string.Empty;


        //user fills these in

        [Required]
        [Display(Name = "Issue Type")]
        public string IssueType { get; set; } = "Damage";

        public decimal Cost { get; set; }

        [Required]
        public string Severity { get; set; } = "Medium";

        [Required(ErrorMessage = "Please describe the issue.")]
        [Display(Name = "Issue Description")]
        public string IssueDescription { get; set; } = string.Empty;

        public List<IFormFile>? Files { get; set; }
    }




    public class CollegeDashboardViewModel
    {
        public string CollegeName { get; set; } = string.Empty;

        public int TotalAssets { get; set; }
        public int PendingMaintenance { get; set; }
        public int IssuesReported { get; set; }
        public int ResolvedIssues { get; set; }

        public List<RecentSubmission> RecentSubmissions { get; set; } = new();

        public List<CollegeAssetRow> Assets { get; set; } = new();
        public string Name { get; set; } = string.Empty;
        public string AdminCollegeName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    //public class CollegeAssetRow
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public string Type { get; set; } = string.Empty;
    //    public string LocationName {  get; set; } = string.Empty;
    //    public string Status {  get; set; } = string.Empty;
    //    public decimal Value { get; set; }
    //}



    public class RecentSubmission
    {
        public string Type { get; set; } = string.Empty; // "Asset Upload" | "Issue Report"
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty;
        public string IssueType {  get; set; } = string.Empty;
    }
    public class MySubmissionsViewModel
    {
        public List<CollegeAssetRow> Assets { get; set; } = new();
        public List<RecentSubmission> Issues { get; set; } = new();

        public int TotalAssets { get; set; }
        public int ActiveAssets { get; set; }
        public int DamagedAssets { get; set; }
        public int PendingAssets { get; set; }
        public int UnderMaintenanceCount { get; set; }
    }
}
