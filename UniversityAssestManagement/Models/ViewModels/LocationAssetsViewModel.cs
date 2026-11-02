namespace UniversityAssetsManagement.Models.ViewModels
{
    public class LocationAssetsViewModel
    {
        public int locationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public string CollegeName {  get; set; } = string.Empty;



        public int TotalAssets { get; set; }
        public int ActiveAssets { get; set; }
        public int DamagedAssets { get; set; }
        public int UnderMaintenance {  get; set; }

        public decimal TotalValue { get; set; }


        public List<AssetCollegeRowViewModel> Assets { get; set; } = new();






    }
}
