namespace UniversityAssestManagement.Models.ViewModels
{
    public class ReportsViewModel
    {

        public decimal TotalAssetValue { get; set; }
        public string MostActiveCollege { get; set; }=string.Empty;
        public int MostActiveCount { get; set; }
        public int TotalDamaged {  get; set; }


        public List<CollegeAssetsSummary> AssetByCollege { get; set; } = new();

        public int GrandTotal { get; set; }
        public int GrandActive { get; set; }
        public int GrandDamaged { get; set; }

        public List<DamagedAssetRow> DamagedAssets { get; set; } = new();
        public AssetValueBreakdown ValueBreakdown { get; set; } =new();

    }

    public class DamagedAssetRow
    {
        public string AssetName { get; set; } = string.Empty;
        public string CollegeName {  get; set; } = string.Empty;
        public string Severity {  get; set; } = string.Empty;
        public DateTime ReportedDate { get; set; }
    }


    public class AssetValueBreakdown
    {
        public decimal UniqueValue { get; set; }
        public decimal BulkValue { get; set; }
        public decimal Total {  get; set; }
        public int UniquePercent => Total == 0 ? 0 : (int)Math.Round(UniqueValue / Total * 100);
        public int BulkPercent => Total == 0 ? 0 : (int)Math.Round(BulkValue / Total * 100);
    }

}
