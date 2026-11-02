
using UniversityAssestManagement.Models;
using UniversityAssestManagement.Models.ViewModels;
using UniversityAssestManagement.Repositories;
using UniversityAssetsManagement.Models.ViewModels;
using UniversityAssetsManagement.Repositories;
using static UniversityAssestManagement.Models.ViewModels.MaintenanceViewModel;

namespace UniversityAssestManagement.Services
{

   
    public class MaintenanceService : IMaintenanceService
    {

        private readonly IMaintenanceRepository _repo;
        private readonly ITechnicianRepository _techRepo;
        private readonly IAssetRepository _assetRepo;

        public MaintenanceService(IMaintenanceRepository repo, ITechnicianRepository techRepo, IAssetRepository assetRepo)
        {
            _repo = repo;
            _techRepo = techRepo;
            _assetRepo = assetRepo;
        }

        public async Task<MaintenanceViewModel> GetAllAsync(int page =1)
        {
            const int pageSize = 10;
            var totalcost = await _repo.TotalCostAsync();
            var totalPending = await _repo.TotalPendingAsync();
            var totalFixed = await _repo.TotalFixedAsync();
            var totalRecord = await _repo.TotalRecordAsync();
            var records = await _repo.GetAllAsync(page, pageSize);
            return new MaintenanceViewModel
            {

                TotalRecords = totalRecord,
                FixedCount = totalFixed,
                PendingCount = totalPending,
                TotalCost = totalcost,

                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalRecord / (double)pageSize),

                Records = records.Select(m => new MaintenanceViewModel.MaintenanceRow
                {
                    Id = m.Id,
                    AssetName = m.Asset.Name,
                    CollegName = m.Asset.College.Name.Replace("College of", ""),
                    IssueDescription = m.IssueDescription,
                    Cost = m.Cost,
                    Status = m.Status,
                    Date = m.Date,  
                    ResolutionNotes = m.ResolutionNotes,
                    ResolvedAt = m.ResolvedAt,
       
                    TechnicianName = m.Technician != null ? m.Technician.FullName : m.TechnicianName ?? "--",
                }).ToList(),

            };
            
        }


        public async Task<MySubmissionsViewModel> GetMySubmissionAsync(int collegeId)
        {
            var assets = await _assetRepo.GetByCollegeAsync(collegeId);
           var report = await _repo.GetAllReportSubmissionByCollege(collegeId);

            return new MySubmissionsViewModel
            {
                TotalAssets = assets.Count,
               
                PendingAssets = assets.Count( a => a.MaintenanceRecords.Any(m => m.Status == MaintenanceStatus.Pending)),
                ActiveAssets = assets.Count(a => a.Status == AssetStatus.Active),
                DamagedAssets = assets.Count(a => a.Status == AssetStatus.Damaged),
                UnderMaintenanceCount = assets.Count(a => a.Status == AssetStatus.UnderMaintenance),

                Assets = assets.Select(a => new CollegeAssetRow
                {
                    Id = a.Id,
                    Name = a.Name,
                    Type = a.Type,
                    LocationName = a.Location.Name,
                    Status = a.Status,
                    Value = a.Value,
                }).ToList(),


                Issues = report
                   .Select( m => new RecentSubmission
                    {

                        Type = "Issue Report",
                        Name = $"{m.Asset.Name} - {m.IssueDescription}",
                        Date = m.Date,
                        Status = m.Status,
                        IssueType = m.IssueType,
                    })
                    .OrderByDescending(r => r.Date)
                    .ToList(),

            };
        }




        //update cost
        public async Task<UpdateMaintenanceViewModel?> GetForUpdateAsync(int id)
        {
          
            var record = await _repo.GetByIdAsync(id);
            if (record is null) return null;

            return new UpdateMaintenanceViewModel
            {
                Id = record.Id,
                AssetName = record.Asset.Name,
                IssueDescription = record.IssueDescription,
                CollegeName = record.Asset.College.Name.Replace("College of", ""),
                Date = record.Date,
                Status = record.Status,
                Cost = record.Cost,
                TechnicianName = record.TechnicianName,
                ResolutionNotes = record.ResolutionNotes,
            };

        }

        public async Task UpdateMaintenanceAsync(UpdateMaintenanceViewModel model)
        {
            var record = await _repo.GetByIdAsync(model.Id)
                 ?? throw new KeyNotFoundException($"Maintenance record {model.Id} not found.");

            record.Status = model.Status;
            record.Cost = model.Cost;
            record.TechnicianName = model.TechnicianName;
            record.ResolutionNotes = model.ResolutionNotes;


            if (model.Status == MaintenanceStatus.Fixed && record.ResolvedAt is null)
                record.ResolvedAt = DateTime.UtcNow;

            if (model.Status == MaintenanceStatus.Pending)
                record.ResolvedAt = null;

            var asset =await _assetRepo.GetByIdAsync(record.AssetId);

            if(asset is not null)
            {
                asset.Status = model.Status switch
                {
                    MaintenanceStatus.Fixed => AssetStatus.Active,
                    MaintenanceStatus.Pending => AssetStatus.UnderMaintenance,
                    MaintenanceStatus.UnderMaintenance => AssetStatus.UnderMaintenance,

                    _ => AssetStatus.UnderMaintenance,
                };
                await _assetRepo.UpdateAsync(asset);


            }

            await _repo.UpdateAsync(record);


        }

        public async Task UpdateStatusAsync(int id, string status)
         => await _repo.UpdateStatusAsync(id, status);


        public async Task<AssignTechnicianViewModel?> GetForAssignmentAsync(int recordId)
        {
            var record = await _repo.GetByIdWithDetailsAsync(recordId);
            if(record is null) return null;
            
            var technicians = await _techRepo.GetAllTechniciansAsync();

            return new AssignTechnicianViewModel
            {
                RecordId = record.Id,
                AssetName = record.Asset.Name,
                CollegeName = record.Asset.Name,
                LocationName = record.Asset.Location.Name,
                IssueDescription = record.IssueDescription,
                CurrentStatus = record.Status,
                TicketId = $"#ISS{record.Id:D4}",

                Technicians = technicians.Where(t => t.IsActive)
                .Select(t => new TechnicianDropdownItem
                {
                    Id = t.Id,
                   FullName = t.FullName,
                    Speciality = t.Specialty,
                    Email = t.Email
                }).ToList(),

            };
        }

        public async Task AssignTechnicianAsync(int recordId, int technicianId, string assignedBy) =>
            await _repo.AssignTechnicianAsync(recordId, technicianId, assignedBy);


    }
}
