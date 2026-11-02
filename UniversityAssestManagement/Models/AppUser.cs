using Microsoft.AspNetCore.Identity;

namespace UniversityAssestManagement.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        public int? CollegeId { get; set; }
        public string? CollegeName { get; set; }

        // Account management
        public bool IsActive { get; set; } = true;
        public bool MustChangePassword { get; set; } = true;

        // Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }  // email of the SuperAdmin who created it

        // Navigation
        public College? College { get; set; }
    }
}