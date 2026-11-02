namespace UniversityAssestManagement.Models
{
    public class Location
    {

        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int CollegeId { get; set; }

        public College College { get; set; } = null!;
        public ICollection<Asset> Assets { get; set; } = new List<Asset>();


    }
}
