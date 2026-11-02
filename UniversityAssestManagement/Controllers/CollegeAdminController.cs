using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityAssestManagement.Data;
using UniversityAssestManagement.Models;
using UniversityAssestManagement.Models.ViewModels;
using UniversityAssestManagement.Repositories;
using UniversityAssestManagement.Services;
using UniversityAssetsManagement.Models.ViewModels;
using UniversityAssetsManagement.Repositories;

namespace UniversityAssetsManagement.Controllers
{
    public class CollegeAdminController : Controller
    {
        private readonly IAssetRepository _assetRepo;
        private readonly ILocationRepository _locationRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMaintenanceService _maintenanceService;

        private readonly AppDbContext _db;
        private readonly IAssetService _service;

        public CollegeAdminController(
            IMaintenanceService maintenanceService, 
            AppDbContext db, UserManager<AppUser> userManager,
            IAssetRepository assetRpo,
            ILocationRepository locationRepo, 
            IAssetService service)
        {
            _db = db;
            _assetRepo = assetRpo;
            _locationRepo = locationRepo;
            _userManager = userManager;
            _service = service;
            _maintenanceService = maintenanceService;
        }

        private async Task<int?> GetCurrentCollegeIdAsync()
        {
            var claim = User.FindFirst("CollegeId");
            if (claim != null) return int.Parse(claim.Value);
            var user = await _userManager.GetUserAsync(User);

            return user?.CollegeId;
        }


        //college  /  Index
        public async Task<IActionResult> Index()
        {

            var  collegeId = await GetCurrentCollegeIdAsync();
            if (collegeId is null) return Forbid();

            var assets = await _assetRepo.GetByCollegeAsync(collegeId.Value);

            var vm = new CollegeDashboardViewModel
            {
                CollegeName = User.FindFirst("CollegeName")?.Value ?? "Your College",
                TotalAssets = assets.Count,
                PendingMaintenance = assets.Count(a => a.Status == AssetStatus.UnderMaintenance),
                IssuesReported = assets.Count(a => a.Status == AssetStatus.Damaged || a.Status ==AssetStatus.Missing
                || a.Status == AssetStatus.Malfunction || a.Status == AssetStatus.Missing || a.Status == AssetStatus.MaintenanceNeeded || a.Status== AssetStatus.UnderMaintenance),
                ResolvedIssues = assets.SelectMany(a => a.MaintenanceRecords)
                    .Count(m => m.Status == "Fixed"),
             //AdminCollegeName= assets.
               

                RecentSubmissions = assets
                            .OrderByDescending(a => a.CreatedAt)
                            .Take(8)
                            .Select(a => new RecentSubmission
                            {
                                Type = "Asset Upload",
                                Name = a.Name,
                                Date = a.CreatedAt,
                                Status = a.Status,

                            }).ToList()
            };



            return View(vm);
        }



        // get / college / uploadassets
        public async Task<IActionResult> UploadAsset()
        {

            var collegeId = await GetCurrentCollegeIdAsync();
            if(collegeId is null) return Forbid();


            var vm = new UploadAssetViewModel
            {
                Locations = (await _locationRepo.GetByCollegeIdAsync(collegeId.Value))
                    .Select(l => new LocationDropdownItem
                    {
                        Id = l.Id,
                        Name = l.Name,
                    }).ToList()
            };

            return View(vm);
        }


        //post / college / uploadasset

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadAsset (UploadAssetViewModel model)
        {
           

            var collegeId = await GetCurrentCollegeIdAsync ();

            if (collegeId is null) return Forbid ();

            if (model.Type == AssetType.Bulk && (model.Quantity is null || model.Quantity < 1))
                ModelState.AddModelError("Quantity", "Quantity is required for bulk assets.");

            if (model.LocationOption == "select" && model.LocationId == null)
                ModelState.AddModelError("LocationId", "Please select a location");

            if (model.LocationOption == "manual" && string.IsNullOrWhiteSpace(model.NewLocation))
                ModelState.AddModelError("NewLocation", "Please enter a location name");

            if (!ModelState.IsValid)
             {
                    model.Locations = (await _locationRepo.GetByCollegeIdAsync(collegeId.Value))
                            .Select(l => new LocationDropdownItem{ Id =l.Id, Name = l.Name})
                            .ToList();

                    return View(model);
             }

            int resolvedLocationId;

            if(model.LocationOption == "manual")
            {
                var newLocation = new Location
                {
                    Name = model.NewLocation!.Trim(),
                    CollegeId = collegeId.Value,
                };

                _db.Locations.Add(newLocation);
                await _db.SaveChangesAsync();
                resolvedLocationId = newLocation.Id;
            }
            else
            {

                var belongsToCollege = await _db.Locations
                    .AnyAsync(l => l.Id == model.LocationId && l.CollegeId == collegeId.Value);

                if (!belongsToCollege)
                {
                    ModelState.AddModelError("LocationId", "Invalid location selected.");
                    model.Locations = (await _locationRepo.GetByCollegeIdAsync(collegeId.Value))
                        .Select(l => new LocationDropdownItem { Id = l.Id, Name = l.Name })
                        .ToList();

                    return View(model);
                }
                resolvedLocationId = model.LocationId!.Value;
            }



            var asset = new Asset
            {

                Name = model.Type == AssetType.Bulk
                            ? $"{model.Name} ({model.Quantity})" : model.Name,
                Type = model.Type,
                Value = model.Value,
                Status = AssetStatus.Active,
                Description=model.Description,
                Quantity = model.Quantity,
                CollegeId = collegeId.Value,
                LocationId = resolvedLocationId,
                CreatedAt = DateTime.UtcNow,
                UploadedBy = User.Identity?.Name,
            };


            _db.Assets.Add(asset);
            await _db.SaveChangesAsync();


            TempData["Success"] = $"Asset \"{model.Name}\" Uploaded Successfully" +
                (model.Files?.Any() == true ? $"with {model.Files.Count} document(s)." : ".");

            return RedirectToAction(nameof(UploadAsset));
        }


        //get /college / reportissue

        public async Task<IActionResult> ReportIssues( int assetId)
        {
            var collegeId = await GetCurrentCollegeIdAsync();
            if (collegeId is null) return Forbid();

            var asset = await _db.Assets
                .Include(a => a.Location)
                .FirstOrDefaultAsync(a => a.Id == assetId && a.CollegeId == collegeId.Value);

            if(asset is null) return NotFound();



            var vm = new ReportIssueViewModel
            {
                AssetId = asset.Id,
                Name = asset.Name,
                LocationName = asset.Location.Name,
            };

            return View(vm);
        }



        //post / college / reportIssue 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> ReportIssues (ReportIssueViewModel model)
        {
            var collegeId = await GetCurrentCollegeIdAsync();
            if (collegeId is null) return Forbid();

            if (!ModelState.IsValid) return View(model);


            var asset = await _db.Assets
                .Include(a => a.Location)
                .Include(a => a.MaintenanceRecords)
                .FirstOrDefaultAsync(a => a.Id == model.AssetId && a.CollegeId == collegeId.Value);

            if (asset is null)
            {
                ModelState.AddModelError("", "Asset not found in your college's record");
                return View(model);
            }


            //asset.Status = model.Severity is "Critical" or "High"
            //    ? AssetStatus.Damaged : AssetStatus.UnderMaintenance;
            asset.Status = model.IssueType;


            if(asset.Type == AssetType.Bulk)
            {

            //    int damaged = AssetStatus.Damaged ?? 0;

            }

            var record = new MaintenanceRecord
            {
                AssetId = asset.Id,
                IssueDescription = model.IssueDescription,
                //$"{model.IssueDescription} " +
                //$"(Location:{asset.Location.Name})",
                Cost = 0m,
                Status = "Pending",
                IssueType = model.IssueType,
                Severity = model.Severity,

                Date = DateTime.UtcNow,
            };

            _db.MaintenanceRecords.Add(record);
            await _db.SaveChangesAsync();

            var tickedId = $"#ISS{record.Id:D4}";
            TempData["Success"] = $"Issue report for \"{asset.Name}\" submitted successfully. Ticket ID: {tickedId}";

            return RedirectToAction("Assets", "CollegeAdmin");
        }
    


        // college / submission

        public async Task<IActionResult> Submission()
        {
            var collegeId = await GetCurrentCollegeIdAsync();

            if (collegeId is null) return Forbid();

            var vm = await _maintenanceService.GetMySubmissionAsync(collegeId.Value);
            ViewData["ActivePage"] = "CSubmission";

            return View(vm);

        }

       

        //get all assets uploaded by colleges
        public async Task<IActionResult> Assets()
        {
            var collegeId = await GetCurrentCollegeIdAsync();
            if (collegeId is null) return Forbid();

            var vm = await _service.GetCollegeAssetAsync(collegeId.Value);
            return View(vm);

           
        }

        //college admin details
        public async Task<IActionResult> CollegeAdminDetails()
        {

            var collegeId = await GetCurrentCollegeIdAsync();

            if (collegeId is null) return Forbid();

            var user = await _db.Users
                .Include(u => u.College)
                .FirstOrDefaultAsync(u => u.CollegeId == collegeId);
                    
            if(user == null) return NotFound();

            var model = new CollegeAdminDetail
            {
                CollegeName = user.College?.Name ?? "",
                Email = user.Email ?? "",
                Name = user.FullName,
            };

            return View(model); 
              
               
        }

      // get / loactions
        public async Task<IActionResult> Locations()
        {
            var collegeId = await GetCurrentCollegeIdAsync();
            if (collegeId is null) return Forbid();

            var location = await _locationRepo.GetByCollegeIdAsync(collegeId.Value);

            ViewBag.CollegeName = User.FindFirst("CollegeName")?.Value ?? "Your College";

            return View(location);


        }

        //Add Location to college admin 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLocation(string name)
        {
            var collegeId = await GetCurrentCollegeIdAsync();
            if(collegeId is null) return Forbid();

            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["Error"] = "Location name cannot be empty.";
                return RedirectToAction(nameof(Locations));

            }

            var exists = await _locationRepo.ExistsAsync(collegeId.Value, name);
            if (exists) {

                TempData["Error"] = $"A location named \"{ name}\" already exists. ";
                return RedirectToAction(nameof(Locations));
            }
            var location = new Location
            {
                Name = name.Trim(),
                CollegeId = collegeId.Value,
            };

            await _locationRepo.AddAsync(location);
            TempData["Success"] = $"Location \"{name}\" added successfully";
            return RedirectToAction(nameof(Locations));

        }


        //Get  /  asset  /  details
        public async Task<IActionResult> AssetDetails(int id)
        {
            var collegeId = await GetCurrentCollegeIdAsync();
        
            if(collegeId == null) return Forbid();

            var vm = await _service.GetCollegeAssetDetailsAsync(id, collegeId.Value);
            if(vm == null) return NotFound();
            return View(vm);


        }



        //get / college / delete assets
        public async Task<IActionResult> Delete(int id)
        {
            var collegeId = await GetCurrentCollegeIdAsync();
            if (collegeId is null) return Forbid();

            var asset = await _service.GetCollegeAssetDetailsAsync(id, collegeId.Value);
            if (asset is null) return NotFound();
            return View(asset);
        }   



        // post / college / delete / asset
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed (int id)
        {
            var collegeId = await GetCurrentCollegeIdAsync();
            if (collegeId is null) return Forbid();

            var asset = await _service.GetCollegeAssetDetailsAsync(id, collegeId.Value);
            if(asset is null) return NotFound();

            await _service.DeleteAsync(id);
            TempData["Success"] = $"\"{asset.Name}\" deleted successfully.";

            return RedirectToAction(nameof(Assets));
        }






        //get /  college  /  edit  /  asset
        public async Task<IActionResult> Edit(int id) {

            var collegeId = await GetCurrentCollegeIdAsync();
            if(collegeId is null) return Forbid();


            var vm = await _service.GetForEditAsync(id, collegeId.Value);
            if(vm is null) return NotFound();

            return View(vm);

        }
        // post / college / edit / asset
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditAssetViewModel model)
        {

            var collegeId = await GetCurrentCollegeIdAsync();
            if (collegeId is null) return Forbid();

            if(id != model.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                var locations = await _locationRepo.GetByCollegeIdAsync(collegeId.Value);
                model.Locations = locations.Select(l => new LocationDropdownItem
                {
                    Id = l.Id,
                    Name = l.Name,
                }).ToList();

                return View(model);
            }
            try
            {
                await _service.UpdateCollegeAssetAsync(collegeId.Value, model);
                TempData["Success"] = $"\"{model.Name}\" updated successfully.";
                return RedirectToAction(nameof(Assets));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }


        }



        //post / collegeadmin / editlocaion

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> EditLocation(int id, string name)
        {
            var collegeId = await GetCurrentCollegeIdAsync();
            if (collegeId is null) return Forbid();

            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["Error"] = "Location name cannot be empty.";
                return RedirectToAction(nameof(Locations));
            }

            var location = await _locationRepo.GetByIdAsync(id);

            if (location is null || location.CollegeId != collegeId.Value)
                return NotFound();

            var duplicate = await _db.Locations
                .AnyAsync(l => l.CollegeId == collegeId.Value && l.Name.ToLower() == name.ToLower().Trim() && l.Id != id);

            if (duplicate)
            {
                TempData["Error"] = $"A location named \"{name}\" already exists.";
                return RedirectToAction(nameof(Locations));
            }


            location.Name = name.Trim();
            await _locationRepo.UpdateAsync(location);

            TempData["Success"] = $"Location renamed to \"{name}\" successfully.";

            return RedirectToAction(nameof(Locations));
        }









        //asset at location
        public async Task<IActionResult> LocationAssets(int locationId)
        {
            var collegeId= await GetCurrentCollegeIdAsync();
            if (collegeId is null) return Forbid();

            var vm = await _service.GetByLocationAsync(locationId, collegeId.Value);
            if(vm is null) return NotFound();

            return View(vm);
        }








    }
}
