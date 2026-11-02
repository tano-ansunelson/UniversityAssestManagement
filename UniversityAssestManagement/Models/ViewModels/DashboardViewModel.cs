namespace UniversityAssestManagement.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalAssets { get; set; }
        public int TotalColleges { get; set; }
        public int DamagedAssets { get; set; }
        public int UnderMaintenance { get; set; }


        public List<Asset> RecentAssets { get; set; } = new();
        public List<CollegeAssetsSummary> CollegeSummaries { get; set; } = new();

    }

public class CollegeAssetsSummary
    {
        public string College {  get; set; } = string.Empty;
        public int Total { get; set; }
        public int Active { get; set; }
        public int Damaged { get; set; }
        public int Maintenance { get; set; }

        //bar
        public int BarPercentage { get; set; }


    }
    

}
