namespace UniversityAssestManagement.Models.ViewModels
{
    public class AssetsViewModel
    {
        public List<Asset> Assets { get; set; } = new();

        public string SearchTerm  { get; set; } = string.Empty;

        public int FilteredCount { get; set; } 
        public int TotalCount { get; set; }
        public string? Status { get; set; }
        public string? Type { get; set; }


        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }



    }


    public class AssetDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public DateTime CreatedAt { get; set; }


        public string CollegeName {  get; set; } = string.Empty;
        public string CollegeCode {  get; set; } = string.Empty;
        public string ColorClass {  get; set; } = string.Empty;
        public string LocationName {  get; set; } = string.Empty;
        public List<MaintenanceRecord> MaintenanceRecords { get; set; } = new();
        public int TotalMaintenance {  get; set; }
        public decimal TotalRepairCost { get; set; }
    }



    public class AddAssetsViewModel
    {

        public string Name { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public decimal Value { get; set; }

        public string Status { get; set; } = AssetStatus.Active;

        public int CollegeId { get; set; }

        public int LocationId { get; set; }

        public string? NewLocation { get; set; }

        public string? LocationOption { get; set; }


    }



    public class CollegeAssetViewModel
    {
        public string Name {  get; set; } = string.Empty;
        public string Type { get; set; }= string.Empty;
        public string Status { get; set; } = string.Empty;

        public string LocationName { get; set; } = string.Empty;

    }




}
