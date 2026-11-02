using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityAssestManagement.Data;
using UniversityAssestManagement.Models;

namespace UniversityAssestManagement.Repositories
{
    public class MaintenanceRepository : IMaintenanceRepository
    {
        private readonly AppDbContext _db;

        public MaintenanceRepository(AppDbContext db)
        {
            _db = db;
        }

        private IQueryable<MaintenanceRecord> BaseQuery() => _db.MaintenanceRecords
            .Include(m => m.Asset)
               .ThenInclude(a => a.College)
            .Include(m => m.Asset)
                .ThenInclude(a => a.Location)
            .Include(m => m.Technician)
            .AsNoTracking();

        public async Task<List<MaintenanceRecord>> GetAllAsync(int page, int pageSize) =>
            await BaseQuery()
            .OrderBy(m => m.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();



        public async Task<decimal> TotalCostAsync() => await
            _db.MaintenanceRecords
                .SumAsync(t => t.Cost);





        public async Task<int> TotalFixedAsync()
        {
            return await _db.MaintenanceRecords
                 .Where(m => m.Status == MaintenanceStatus.Fixed)
                 .CountAsync();
        }


        public async Task<int> TotalPendingAsync()
        {
            return await _db.MaintenanceRecords
                   .Where(m => m.Status == MaintenanceStatus.Pending)
                   .CountAsync();

        }

        public async Task<int> TotalRecordAsync()
        {
            return await _db.MaintenanceRecords.CountAsync();
        }

        public async Task<List<MaintenanceRecord>> GetAllReportSubmissionByCollege(int collegeId) =>
               await _db.MaintenanceRecords
                        .Include(m => m.Asset)
                            .ThenInclude(a => a.College)
                        .Include(m => m.Asset)
                            .ThenInclude(a => a.Location)

                    .Where(m => m.Asset.CollegeId == collegeId)
                    .OrderByDescending(m => m.Date)
                    .ToListAsync();

        public async Task<MaintenanceRecord?> GetByIdAsync(int id) =>
            await _db.MaintenanceRecords
                    .Include(m => m.Asset)
                        .ThenInclude(a => a.College)
                   
                    .FirstOrDefaultAsync(m => m.Id == id );


        public async Task UpdateStatusAsync(int id, string status)
        {
            var record = await _db.MaintenanceRecords.FindAsync(id)
                  ?? throw new KeyNotFoundException($"Maintenance record {id} not found.");

            record.Status = status;
            var asset = await _db.Assets.FindAsync(record.AssetId);
            if (asset is not null)
            {
                asset.Status = status switch
                {
                    MaintenanceStatus.Fixed => AssetStatus.Active,
                    MaintenanceStatus.Pending => AssetStatus.UnderMaintenance,
                    _ => AssetStatus.UnderMaintenance,
                };
            }

            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(MaintenanceRecord record)
        {
            _db.MaintenanceRecords.Update(record);
            await _db.SaveChangesAsync();
        }

        public async Task<MaintenanceRecord?> GetByIdWithDetailsAsync(int id) =>
                await _db.MaintenanceRecords
            .Include(m => m.Asset)
                .ThenInclude(a => a.College)
            .Include(m => m.Asset)
                .ThenInclude(a => a.Location)
            .Include(m => m.Technician)
            .FirstOrDefaultAsync(m => m.Id == id);


        public async Task AssignTechnicianAsync(int recordId, int technicianId, string assignedBy)
        {
            var record = await _db.MaintenanceRecords.FindAsync(recordId)
                ?? throw new KeyNotFoundException($"Maintenance record {recordId} not found.");

            record.TechnicianId = technicianId;
            record.AssignedAt = DateTime.UtcNow;
            record.AssignedBy = assignedBy;
            record.Status = MaintenanceStatus.Assigned;


            var asset = await _db.Assets.FindAsync(record.AssetId);
            if (asset is not null)
            {
                asset.Status = AssetStatus.UnderMaintenance;
            }
            await _db.SaveChangesAsync();

        }
    }
}
 