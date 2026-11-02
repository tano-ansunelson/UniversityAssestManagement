using System.ComponentModel.DataAnnotations;

namespace UniversityAssetsManagement.Models.ViewModels
{
    public class TechnicianViewModel
    {
        public class TechnicianListViewModel
        {
            public List<TechnicianRow> Technicians { get; set; } = new();
            public int TotalActive { get; set; }
            public int TotalInactive { get; set; }
        }



        public class TechnicianRow
        {
            public int Id { get; set; }
            public string FullName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public string Speciality { get; set; } = string.Empty;
            public bool IsActive { get; set; }

            public DateTime CreateAt { get; set; }
            public int JobsAssigned { get; set; }


        }

        public class SaveTechnicianViewModel
        {
            public int Id { get; set; } // 0 =create, >0 = edit

            [Required(ErrorMessage = "Full Name is required")]
            [Display(Name = "Full Name")]
            public string FullName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid Email Address")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Phone Number is required")]
            public string PhoneNumber { get; set; } = string.Empty;

            [Required(ErrorMessage = "Speciality is required")]
            [Display(Name = "Speciality")]
            public string Speciality { get; set; } = string.Empty;
            public bool IsActive { get; set; } = true;
        }


    }
}
