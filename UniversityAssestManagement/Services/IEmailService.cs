namespace UniversityAssetsManagement.Services
{
    public interface IEmailService
    {
        Task SendAssignmentEmailAsync(AssignmentEmailModel model);


    }

    public class AssignmentEmailModel
    {
         public string TechnicianName { get; set; } = string.Empty;
         public string TechnicianEmail { get; set; } = string.Empty;
        public string AssetName { get; set; } = string.Empty;
        public string CollegeName { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;

        public string IssueDescription { get; set; } = string.Empty;
        public string AssignedBy { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
        public string TicketId { get; set; } = string.Empty;



    }
    
        
    
}
