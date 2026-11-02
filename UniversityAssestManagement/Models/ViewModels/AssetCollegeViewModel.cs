namespace UniversityAssetsManagement.Models.ViewModels
{
    public class AssetCollegeViewModel
    {
     

        public List<AssetCollegeRowViewModel> CollegeAssets { get; set; } = new();
    }



   


 public class AssetCollegeRowViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Value { get; set; }

}


    public class CollegeAdminDetail
    {
        public string Name { get; set; } = string.Empty;
        public string CollegeName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
    
}
