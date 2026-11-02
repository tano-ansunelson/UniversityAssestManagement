using UniversityAssestManagement.Models.ViewModels;


namespace UniversityAssestManagement.Services
{
    public interface ICollegeService
    {
        Task<CollegeViewModel> GetAllAsync();

        Task<CollegeDetailsViewModel?> GetDetailsAsync(int id);

        Task<bool> CodeExistsAsync(string code);
        Task AddAsync(AddCollegeViewModel model);

        Task DeleteAsync(int id);

    }
}
