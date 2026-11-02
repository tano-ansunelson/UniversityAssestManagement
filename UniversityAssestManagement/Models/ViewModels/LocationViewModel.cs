namespace UniversityAssestManagement.Models.ViewModels
{
    public class LocationViewModel
    {
        public List<CollegeLocationGroup> Groups { get; set; } = new();
    }

    public class CollegeLocationGroup
    {
        public string CollegeName { get; set; } = string.Empty;
        public string CollegeCode { get; set; } = string.Empty;
        public int TotalAssets { get; set; }
        public List<LocationSummary> Locations { get; set; } = new();
    }


    public class LocationSummary
    {
        public int Id { get; set; }
        public string Name { get; set; } =string.Empty;
        public int AssetCount { get; set; }
    }
}
