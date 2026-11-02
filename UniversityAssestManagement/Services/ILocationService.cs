using UniversityAssestManagement.Models;
using UniversityAssestManagement.Models.ViewModels;

namespace UniversityAssetsManagement.Services
{
    public interface ILocationService
    {

        Task<LocationViewModel> GetAllGroupedAsync();

        Task<Location?> GetByIdAsync(int id);
        //Task<Location?> AddAsync(Location location);
    }
}
