using UniversityAssestManagement.Models;

namespace UniversityAssestManagement.Repositories
{
    public interface IMaintenanceRepository
    {

        Task <List<MaintenanceRecord> > GetAllAsync(int page, int pageSize);
        Task<int> TotalRecordAsync();
        Task<int> TotalPendingAsync();
        Task<int> TotalFixedAsync ();

        Task<decimal> TotalCostAsync();



        Task<List<MaintenanceRecord>> GetAllReportSubmissionByCollege(int collegeId);
        Task<MaintenanceRecord?> GetByIdAsync(int id);

        Task UpdateStatusAsync(int id, string status);



        //update cost

       // Task<MaintenanceRecord?> GetByIdAsync(int id);
        Task UpdateAsync (MaintenanceRecord record);


        Task<MaintenanceRecord?> GetByIdWithDetailsAsync(int id);
        Task AssignTechnicianAsync(int recordId, int technicianId, string assignedBy);

    }
}
