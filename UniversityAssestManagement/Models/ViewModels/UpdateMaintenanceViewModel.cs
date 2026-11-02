using System.ComponentModel.DataAnnotations;
using UniversityAssestManagement.Models;

namespace UniversityAssetsManagement.Models.ViewModels
{
    public class UpdateMaintenanceViewModel
    {

        public int Id { get; set; }
        public string AssetName { get; set; } = string.Empty;

        public string IssueDescription { get; set; } = string.Empty;

        public string CollegeName {  get; set; } = string.Empty;
        public DateTime Date {  get; set; }

        [Required]
        public string Status { get; set; } = MaintenanceStatus.Pending;
        [Range(0, double.MaxValue,ErrorMessage ="Cost must be a positive number.")]
        public decimal Cost { get; set; }

        [Display(Name ="Technician Name")]
        public string? TechnicianName { get; set; }

        [Display(Name ="Resolution Notes")]
        public string? ResolutionNotes { get; set; }



    }
}
