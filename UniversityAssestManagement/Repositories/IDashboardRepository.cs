using UniversityAssestManagement.Models;

namespace UniversityAssetsManagement.Repositories
{
    public interface IDashboardRepository
    {
        Task<int> GetTotalAssetsAsync();
        Task<int > GetTotalCollegeAsync ();
        Task<int > GetDamagedAssetsAsync();
        Task<int> GetUnderMaintenanceAsync ();
        Task<List<Asset>> GetRecentAssetsAsync( int count = 6);
        Task<List<College>> GetCollegesWithAssetsAsync ();

    }
}
