using UniversityAssestManagement.Models;

namespace UniversityAssetsManagement.Repositories
{
    public interface IReportsRepository
    {

        Task<List<College>> GetCollegeWithAssetsAsync();
        Task<List<Asset>> GetDamagedAssetsAsync();
        Task<decimal> GetTotalAssetValueAsync();
        Task<decimal>GetValueByTypeAsync(string type);
    }
}
