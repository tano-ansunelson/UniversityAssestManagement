using UniversityAssestManagement.Models.ViewModels;
using UniversityAssetsManagement.Models.ViewModels;
using static UniversityAssestManagement.Models.ViewModels.MaintenanceViewModel;

namespace UniversityAssestManagement.Services
{
    public interface IMaintenanceService
    {
        Task<MaintenanceViewModel> GetAllAsync( int page =1);

        Task<MySubmissionsViewModel> GetMySubmissionAsync(int collegeId);

        Task UpdateStatusAsync (int id, string status);


        Task<UpdateMaintenanceViewModel?> GetForUpdateAsync(int id);

        Task UpdateMaintenanceAsync(UpdateMaintenanceViewModel model);

        Task<AssignTechnicianViewModel?> GetForAssignmentAsync(int recordId);
        Task AssignTechnicianAsync (int recordId, int technicianId, string assignedBy);



    }
}
