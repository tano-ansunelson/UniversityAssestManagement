using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityAssestManagement.Data;
using UniversityAssestManagement.Models;
using UniversityAssestManagement.Models.ViewModels;
using UniversityAssestManagement.Repositories;
using UniversityAssetsManagement.Models.ViewModels;
using UniversityAssetsManagement.Repositories;

namespace UniversityAssestManagement.Services
{
    public class AssetService : IAssetService
    {
        private readonly AppDbContext _db;
        private readonly IAssetRepository _repo;
        private readonly IHttpContextAccessor _http;
        private readonly ILocationRepository _loca;


        public AssetService(IAssetRepository repo, IHttpContextAccessor http, ILocationRepository loca, AppDbContext db)
        {
            _repo = repo;
            _http = http;
            _loca = loca;
            _db = db;
        }

        // Reads the CollegeId from the logged-in user's claims
        private int? GetCurrentCollegeId()
        {
            var user = _http.HttpContext?.User;
            if (user is null) return null;

            var claim = user.FindFirst("CollegeId");
            return claim is null ? null : int.Parse(claim.Value);
        }

         
        private bool IsAdmin() => 
            _http.HttpContext?.User.IsInRole("SuperAdmin") == true;

        public async Task<AssetsViewModel> GetAssetsAsync(
           string? term, string? status, string? type, int page = 1)
        {
            const int pageSize = 10;

            List<Asset> assets;
            int totalFiltered;
            int totalCount;

            if (IsAdmin())
            {
                // Fix 2: declare variables outside if/else
                // SuperAdmin — sees all assets
                assets = await _repo.SearchAsync(term, status, type, page, pageSize);
                totalFiltered = await _repo.GetFilteredCountAsync(term, status, type);
                totalCount = await _repo.GetTotalCountAsync();
            }
            else
            {
                // CollegeAdmin — scoped to their college only
                var collegeId = GetCurrentCollegeId();
                assets = await _repo.SearchByCollegeAsync(collegeId, term, status, type, page, pageSize);
                totalFiltered = await _repo.GetFilteredCountByCollegeAsync(collegeId, term, status, type);
                totalCount = totalFiltered; // total is the same as filtered for college users
            }

            // Fix 3: use the assets variable directly, not IsAdmin().assets
            return new AssetsViewModel
            {
                Assets = assets,
                SearchTerm = term ?? string.Empty,
                Status = status,
                Type = type,
                FilteredCount = totalFiltered,
                TotalCount = totalCount,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalFiltered / (double)pageSize),
            };
        }
      

        public async Task DeleteAsync(int id) => await _repo.DeleteAsync(id);
     

        public async Task<bool> ExistsAsync(int id) =>await _repo.ExistsAsync(id);
       

       


        public async Task UpdateAsync(Asset asset) =>
            await _repo.UpdateAsync(asset);
       

        public async Task<AssetDetailsViewModel> GetByIdAsync(int id)
        {
           var asset = await _repo.GetByIdAsync(id);
            if (asset == null) return null;

            return new AssetDetailsViewModel
            {
                Id = asset.Id,
                Name = asset.Name,
                Type = asset.Type,
                Status = asset.Status,
                Value = asset.Value,
                CreatedAt = asset.CreatedAt,
                CollegeName = asset.College.Name,
                CollegeCode = asset.College.Code,
                ColorClass = asset.College.ColorClass,
                LocationName = asset.Location.Name,
                MaintenanceRecords = asset.MaintenanceRecords
                    .OrderByDescending(m => m.Date)
                    .ToList(),
                TotalMaintenance = asset.MaintenanceRecords.Count,

                TotalRepairCost = asset.MaintenanceRecords.Sum(m => m.Cost),
            };
        }

        public async Task CreateAsync(AddAssetsViewModel model)
        {
            var assets = new Asset
            {
                Name = model.Name,
                Type = model.Type,
                Value = model.Value,
                Status = AssetStatus.Active,
                CollegeId = model.CollegeId,
                LocationId = model.LocationId,
                CreatedAt = DateTime.UtcNow,

            };
            await _repo.AddAsync(assets);
         
        }

        public async Task<AssetCollegeViewModel> GetCollegeAssetAsync(int collegeId)
        {
            var asset = await _repo.GetByCollegeAsync(collegeId);

            
      

            return new AssetCollegeViewModel

            {
               


                CollegeAssets = asset.Select(a => new AssetCollegeRowViewModel
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

        public async Task<AssetsDetailsViewModel?> GetCollegeAssetDetailsAsync(int id, int collegeId)
        {
           var asset = await _repo.GetByIdForCollegeAsync(id, collegeId);
            if(asset == null)   return null;

            return new AssetsDetailsViewModel
            {
                Id = asset.Id,
                Name = asset.Name,
                Type = asset.Type,
                Status = asset.Status,
                Value = asset.Value,
                Description=asset.Description,
                CreatedAt = asset.CreatedAt,
                LocationName = asset.Location.Name,
                CollegeName = asset.College.Name,


                MaintenanceRecords = asset.MaintenanceRecords
                        .OrderByDescending(m => m.Date)
                        .Select(m => new CollegeMaintenanceRow
                        {
                            IssueDescription = m.IssueDescription,
                            Cost = m.Cost,
                            Status = m.Status,
                            Date = m.Date,
                        }).ToList(),

                TotalMaintenances = asset.MaintenanceRecords.Count,
                TotalRepairCost = asset.MaintenanceRecords.Sum(m => m.Cost),


            };
        }

        public async Task<EditAssetViewModel?> GetForEditAsync(int id, int collegeId)
        {
           var asset = await _repo.GetByIdForEditAsync(id, collegeId);
            if (asset is null) return null;


            var locations = await _loca.GetByCollegeIdAsync(collegeId);
            return new EditAssetViewModel
            {

                Id = asset.Id,
                Name = asset.Name,
                Type = asset.Type,
                Quantity = asset.Quantity,
                LocationId = asset.LocationId,
                Value = asset.Value,
                Description = asset.Description,
                Locations = locations.Select(l => new LocationDropdownItem
                {
                    Id = l.Id,
                    Name = l.Name,
                }).ToList()
            };
        }

        public async Task UpdateCollegeAssetAsync(int collegeId, EditAssetViewModel model)
        {
            var asset = await _repo.GetByIdForEditAsync(model.Id, collegeId)
                 ?? throw new KeyNotFoundException("Asset not found.");

            asset.Name = model.Name.Trim();
            asset.Type = model.Type;
            asset.Quantity = model.Type == AssetType.Bulk ? model.Quantity : null;
            asset.LocationId = model.LocationId;
            asset.Value = model.Value;
            asset.Description = model.Description;

            await _repo.UpdateAsync(asset);

        }

        public async Task<LocationAssetsViewModel?> GetByLocationAsync(int locationId, int collegeId)
        {
           var locationExists = await _db.Locations
                .AnyAsync(l => l.Id == locationId && l.CollegeId == collegeId);


            if(!locationExists) return null;

            var location = await _db.Locations.FindAsync(locationId);
            var asset = await  _repo.GetByLocationAsync(locationId, collegeId);



            return new LocationAssetsViewModel
            {
                locationId = locationId,
                LocationName = location!.Name,
                CollegeName = asset.FirstOrDefault()?.College.Name ?? "",
                TotalAssets = asset.Count(),
                ActiveAssets = asset.Count(a => a.Status == AssetStatus.Active),
                DamagedAssets = asset.Count(a => a.Status == AssetStatus.Damaged),
                UnderMaintenance = asset.Count(a => a.Status == AssetStatus.UnderMaintenance),
                TotalValue = asset.Sum(a => a.Value),
                Assets = asset.Select(a => new AssetCollegeRowViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    Type = a.Type,
                    LocationName = a.Location.Name,
                    Status = a.Status,
                    Value = a.Value,
                }).ToList(),
            };









        }
    }
}
