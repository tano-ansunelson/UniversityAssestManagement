namespace UniversityAssestManagement.Models.ViewModels
{
    public class CollegeDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }= string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;

        public string ColorClass { get; set; } = string.Empty;


        //cards
        public int TotalAssest { get; set; }
        public int ActiveAssests { get; set; }
        public int DamagedAssets { get; set; }
        public int UnderMaintenance { get; set; }
        public int TotalLocations { get; set; }
        public decimal TotalValue { get; set; }
        public List<CollegeAssetRow> AssestAssets { get; set; } = new();
    }

    public class CollegeAssetRow
    {
        public int Id { get; set; }
        public string Name {set; get; } = string.Empty;
        public string Type {  get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }
}
