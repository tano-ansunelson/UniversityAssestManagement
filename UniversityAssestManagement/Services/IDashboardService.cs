using UniversityAssestManagement.Models.ViewModels;

namespace UniversityAssetsManagement.Services
{
    public interface IDashboardService
    {

        Task<DashboardViewModel> GetDashboardAsync();
    }
}
