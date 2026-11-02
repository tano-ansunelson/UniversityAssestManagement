using UniversityAssestManagement.Models;

namespace UniversityAssetsManagement.Repositories
{
    public interface ILocationRepository
    {
        Task<List<College>> GetAllGroupedAsync();
        Task<Location?> GetByIdAsync(int id);

        Task AddAsync(Location location);

        Task<List<Location>> GetByCollegeIdAsync(int collegeId);
        Task<bool> ExistsAsync(int collegeId, string name);




        //edit location / collegeadmin
        Task UpdateAsync (Location location);
        
    }
}
