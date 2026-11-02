using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityAssestManagement.Data;
using UniversityAssestManagement.Models;

namespace UniversityAssetsManagement.Repositories
{
    public class ReportsRepository : IReportsRepository
    {
        private readonly AppDbContext _db;
        public ReportsRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<College>> GetCollegeWithAssetsAsync() =>
            await _db.Colleges
                .Include(c => c.Assets)
                .AsNoTracking()
                .ToListAsync();


        public async Task<List<Asset>> GetDamagedAssetsAsync() =>
            await _db.Assets
                .Include(a => a.College)
                .Where(a => a.Status == AssetStatus.Damaged)
                .AsNoTracking()
                .OrderByDescending(a => a.Value)
                .Take(5)
                .ToListAsync();


        public async Task<decimal> GetTotalAssetValueAsync() =>
            await _db.Assets.SumAsync(a => (decimal?)a.Value) ?? 0m;


        public async Task<decimal> GetValueByTypeAsync(string type) =>
            await _db.Assets
                .Where(a => a.Type == type)
                .SumAsync(a => (decimal?)a.Value) ?? 0m;
      
    }
}
