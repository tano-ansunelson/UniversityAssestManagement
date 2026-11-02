using UniversityAssestManagement.Models;

namespace UniversityAssetsManagement.Models
{
    public static class TechnicianSpecialties
    {
        public const string Electrical = "Electrical";
        public const string Mechanical = "Mechanical";
        public const string Plumbing = "Plumbing";
        public const string Furniture = "Furniture";
        public const string IT = "IT Equipment";
        public const string General = "General";
       
      
    }



    public class Technician
    {

        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Specialty { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreateAt { get; set; }
        public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = new List<MaintenanceRecord>();
    }
}
