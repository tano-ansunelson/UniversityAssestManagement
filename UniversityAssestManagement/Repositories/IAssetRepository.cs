using UniversityAssestManagement.Models;
using UniversityAssestManagement.Models.ViewModels;

namespace UniversityAssestManagement.Repositories
{
    public interface IAssetRepository
    {
        Task<List<Asset>> GetAllAsync();
        Task<List<Asset>> SearchAsync(string? term, string? status, string? type, int page, int pageSize);
        Task<int> GetFilteredCountAsync(string? term, string? status, string? type);
        Task<Asset?> GetByIdAsync(int id);
        Task<int> GetTotalCountAsync();
        Task<List<Asset>> SearchByCollegeAsync(int? collegeId, string? term, string? status, string? type);



        //Assets
        Task AddAsync(Asset asset);
        Task UpdateAsync(Asset asset);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);


        Task<List<Asset>> SearchByCollegeAsync(
        int? collegeId, string? term, string? status, string? type,int page, int pageSize);
        Task<int> GetFilteredCountByCollegeAsync(int? collegeId, string? term, string? status, string? type);
         

        // Add to IAssetRepository.cs
        Task<List<Asset>> GetByCollegeAsync(int collegeId);
        Task<Asset?> GetByIdForCollegeAsync(int id, int collegeId);


        //edit asset
        Task<Asset?> GetByIdForEditAsync(int id, int collegeId);



        //asset at location

        Task<List<Asset>> GetByLocationAsync(int locationId, int collegeId);

    }
}
