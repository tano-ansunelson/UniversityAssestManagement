using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityAssestManagement.Data;
using UniversityAssestManagement.Models;

namespace UniversityAssetsManagement.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly AppDbContext _db;

        public DashboardRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<College>> GetCollegesWithAssetsAsync() =>
            await _db.Colleges
                .Include(c => c.Assets)
                .AsNoTracking()
                .ToListAsync();
       

        public async Task<int> GetDamagedAssetsAsync()=>
            await _db.Assets
            .Where(a => a.Status == AssetStatus.Damaged)
            .CountAsync();
        

        public async Task<List<Asset>> GetRecentAssetsAsync( int count =6) =>
            await _db.Assets
                .Include(a => a.College)
                .Include(a => a.Location)
                .AsNoTracking()
                .OrderByDescending(a => a.CreatedAt)
                .Take(count)
                .ToListAsync();
        

        public async Task<int> GetTotalAssetsAsync() =>
            await _db.Assets.CountAsync();
       

        public async Task<int> GetTotalCollegeAsync()=>
            await _db.Colleges.CountAsync();
      

        public async Task<int> GetUnderMaintenanceAsync()=>
            await _db.Assets
            .Where(a => a.Status == AssetStatus.UnderMaintenance)
            .CountAsync();
       

       
    }
}
