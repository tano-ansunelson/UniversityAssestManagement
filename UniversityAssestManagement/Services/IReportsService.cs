using UniversityAssestManagement.Models.ViewModels;

namespace UniversityAssetsManagement.Services
{
    public interface IReportsService
    {
        Task<ReportsViewModel> GetReportsAsync();
    }
}
