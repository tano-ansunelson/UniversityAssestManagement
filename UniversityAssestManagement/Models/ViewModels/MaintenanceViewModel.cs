namespace UniversityAssestManagement.Models.ViewModels
{
    public class MaintenanceViewModel
    {

        public int TotalRecords { get; set; }
        public int PendingCount { get; set; }
        public int FixedCount { get; set; }
        public decimal TotalCost { get; set; }





        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public List<MaintenanceRow> Records { get; set; } = new();

        public class MaintenanceRow
        {
            public int Id { get; set; }
            public string AssetName { get; set; } = string.Empty;
            public string CollegName { get; set; } = string.Empty;
            public string IssueDescription { get; set; } = string.Empty;
            public decimal Cost { get; set; }
            public string Status { get; set; } = string.Empty;
            public DateTime Date { get; set; }


            //new 
            public string? TechnicianName {  get; set; }

            public DateTime? ResolvedAt { get; set; }
            public string? ResolutionNotes { get; set; }



        }

    }

  

}
