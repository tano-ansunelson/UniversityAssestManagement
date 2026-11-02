using UniversityAssestManagement.Models;

namespace UniversityAssestManagement.Repositories
{
    public interface ICollegeRepository
    {

        Task<List<College>> GetAllAsync();

        Task<College?> GetDetailsAsync(int id);

        Task<bool> ExistingAsync (int id);

        Task AddAsync (College college);
        Task<bool> CodeExistsAsync (string  code);

        Task DeleteAsync(int id);

    }
}
