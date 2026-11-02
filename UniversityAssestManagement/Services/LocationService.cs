using Microsoft.AspNetCore.Mvc;
using UniversityAssestManagement.Models;
using UniversityAssestManagement.Models.ViewModels;
using UniversityAssetsManagement.Repositories;

namespace UniversityAssetsManagement.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _repo;
        public LocationService(ILocationRepository repo)=> _repo = repo;

        //public async Task<Location?> AddAsync(Location location)
        //{
        //  return await _repo.AddAsync(location);
        //}

        public async Task<LocationViewModel> GetAllGroupedAsync()
        {
           var college= await _repo.GetAllGroupedAsync();

            var groups = college.Select(c => new CollegeLocationGroup
            {
                CollegeName = c.Name,
                CollegeCode = c.Code,
                TotalAssets = c.Locations.Sum(l => l.Assets.Count),
                Locations = c.Locations
                            .OrderBy(l =>l.Name)
                            .Select(l => new LocationSummary
                            {
                                Id = l.Id,
                                Name = l.Name,
                                AssetCount = l.Assets.Count,
                            }).ToList()
            })
                .OrderByDescending(g => g.Locations.Count)
                .ThenByDescending(g => g.TotalAssets)
                .ToList();

return new LocationViewModel { Groups = groups };

        }

        public async Task<Location?> GetByIdAsync(int id)
        {
           return await _repo.GetByIdAsync(id);
        }
    }
}
