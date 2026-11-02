using System.ComponentModel.DataAnnotations;

namespace UniversityAssetsManagement.Models.ViewModels
{
    public class AssignTechnicianViewModel
    {

        public int RecordId { get; set; }
        public string AssetName { get; set; } = string.Empty;
        public string IssueDescription { get; set; } = string.Empty;
       
        public string TechnicianName { get; set; } = string.Empty;
     
       
        public string LocationName { get; set; } = string.Empty;
       public string CurrentStatus { get; set; } = string.Empty;
        public string CollegeName { get; set; } = string.Empty;
       public string TicketId { get; set; } = string.Empty;


        [Required(ErrorMessage = "Please select a technician.")]
        [Display(Name = "Technician")]
        public int TechnicianId { get; set; }

        public List<TechnicianDropdownItem> Technicians { get; set; } = new();

    }

    public class TechnicianDropdownItem
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Speciality { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
