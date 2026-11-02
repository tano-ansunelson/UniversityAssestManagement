using Microsoft.AspNetCore.Mvc;
using UniversityAssestManagement.Models;
using UniversityAssestManagement.Models.ViewModels;
using UniversityAssestManagement.Repositories;
//using UniversityAssestManagement.Models.ViewModels;

namespace UniversityAssestManagement.Services
{
    public class CollegeService : ICollegeService
    {

        private readonly ICollegeRepository _repo;

        public CollegeService(ICollegeRepository repo)
        {
            _repo = repo;
        }

        //add college 
        public async Task AddAsync(AddCollegeViewModel model)
        {
            var college = new College
            {
                Name = model.Name.Trim(),
                Code = model.Code.Trim().ToUpper(),
                Description = model.Description.Trim(),
                ColorClass =model.ColorClass
            };
            await _repo.AddAsync(college);
        }

        //add college exist
        public async Task<bool> CodeExistsAsync(string code)=>
            await _repo.CodeExistsAsync(code);


        public async Task<CollegeViewModel> GetAllAsync()
        {
            var college = await _repo.GetAllAsync();

            var cards = college.Select(c => new CollegeCard
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code,
                Description = c.Description,
                ColorClass = c.ColorClass,
                AssetCount = c.Assets.Count,
                LocationCount = c.Locations.Count,

            })
                .OrderByDescending(g => g.LocationCount)
                .ThenByDescending(g =>g.AssetCount)
                .ToList();


            return new CollegeViewModel { Colleges = cards };
        }

     

        async Task<CollegeDetailsViewModel?> ICollegeService.GetDetailsAsync(int id)
        {

            var college = await _repo.GetDetailsAsync(id);


            if (college == null) return null;

            return new CollegeDetailsViewModel
            {
                Id = college.Id,
                Name = college.Name,
                Code = college.Code,
                Description = college.Description,
                ColorClass = college.ColorClass,
                TotalAssest = college.Assets.Count,
                ActiveAssests = college.Assets.Count(a => a.Status == AssetStatus.Active),
                DamagedAssets = college.Assets.Count(a => a.Status == AssetStatus.Damaged),
                UnderMaintenance = college.Assets.Count(a => a.Status == AssetStatus.UnderMaintenance),
                TotalLocations = college.Locations.Count,
                TotalValue = college.Assets.Sum(a => a.Value),

                AssestAssets = college.Assets
                            .OrderBy(a => a.Name)
                            .Select(a => new CollegeAssetRow
                            {
                                Id = a.Id,
                                Name = a.Name,
                                Type = a.Type,
                                LocationName = a.Location.Name,
                                Status = a.Status,
                                Value = a.Value,
                            }).ToList()
            };

        }




        public async Task DeleteAsync(int id) =>
                await _repo.DeleteAsync(id);
      
    }
}
