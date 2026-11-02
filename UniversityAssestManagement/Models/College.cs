namespace UniversityAssestManagement.Models
{
    public class College
    {

        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description {  get; set; } = string.Empty;

        public string ColorClass { get; set; } = string.Empty;

        public ICollection<Location> Locations { get; set; }    = new List<Location>();
        public ICollection<Asset> Assets { get; set; } = new List<Asset>();



    }
}
