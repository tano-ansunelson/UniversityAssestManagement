using System.ComponentModel.DataAnnotations;

namespace UniversityAssestManagement.Models.ViewModels
{
    public class CreateCollegeUserViewModel
    {
        [Required(ErrorMessage = "Full name is required.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm the password.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please assign a college.")]
        [Display(Name = "College")]
        public int CollegeId { get; set; }

        // Populated from DB for the dropdown
        public List<CollegeDropdownItem> Colleges { get; set; } = new();
    }

    public class CollegeDropdownItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    // ── For the listing page ─────────────────────────────────────────
    public class CollegeUserListViewModel
    {
        public List<CollegeUserRow> Users { get; set; } = new();
    }

    public class CollegeUserRow
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CollegeName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}
