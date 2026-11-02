using static UniversityAssetsManagement.Models.ViewModels.TechnicianViewModel;

namespace UniversityAssetsManagement.Services
{
    public interface ITechnicianService
    {
        Task<TechnicianListViewModel> GetAllAsync();

        Task<SaveTechnicianViewModel?> GetForEditAsync(int id);
        Task<(bool Success, string? Error)> CreateAsync(SaveTechnicianViewModel model);
        Task<(bool Success, string? Error)> UpdateAsync(SaveTechnicianViewModel model);

        Task ToggleActiveAsync(int id);
    }
}
