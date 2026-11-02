using UniversityAssestManagement.Models;
using UniversityAssestManagement.Models.ViewModels;
using UniversityAssetsManagement.Models.ViewModels;

namespace UniversityAssestManagement.Services
{
    public interface IAssetService
    {

   //superAdmin/asset
        Task<AssetsViewModel> GetAssetsAsync(string? term, string? status, string? type, int page =1);
        Task<AssetDetailsViewModel> GetByIdAsync(int id);
        Task CreateAsync(AddAssetsViewModel model);
        Task UpdateAsync(Asset asset);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);


        //collegeAdmin/asset

        Task<AssetCollegeViewModel> GetCollegeAssetAsync(int collegeId);
     
        Task<AssetsDetailsViewModel?> GetCollegeAssetDetailsAsync(int id, int collegeId);


        //edit Assets
        Task<EditAssetViewModel?> GetForEditAsync(int id, int collegeId);
        Task UpdateCollegeAssetAsync(int collegeId, EditAssetViewModel model);


        //asset at location

        Task<LocationAssetsViewModel?> GetByLocationAsync(int locationId, int collegeId);
    }
}
