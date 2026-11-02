using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityAssestManagement.Data;
using UniversityAssestManagement.Models;
using UniversityAssestManagement.Models.ViewModels;

namespace UniversityAssestManagement.Repositories
{
    public class AssetRepository : IAssetRepository
    {

        private readonly AppDbContext _db;

        public AssetRepository(AppDbContext db)=> _db = db;


        private IQueryable<Asset> BaseQuery() => _db.Assets
            .Include(a => a.College)
            .Include(a => a.Location)
            .AsNoTracking();

        public async Task<List<Asset>> GetAllAsync()=>
           await BaseQuery()
                .OrderBy(a => a.Name)
                .ToListAsync();
           
        

    
       

        public  async Task<int> GetTotalCountAsync() => await _db.Assets.CountAsync();
        

        public async Task<List<Asset>> SearchAsync(string? term, string? status, string? type, int page, int pageSize)
        {

            var query =BaseQuery();

            if (!string.IsNullOrWhiteSpace(term))
                query = query.Where(a => a.Name.Contains(term) || a.College.Name.Contains(term) || a.Location.Name.Contains(term));

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(a => a.Status ==status);


            if(!string.IsNullOrWhiteSpace(type))
                query = query.Where(a => a.Type ==type);


            return await query.OrderBy(a => a.Name)
                .Skip((page -1) * pageSize)
                .Take(pageSize)
                .ToListAsync();


        }
        public async Task<int> GetFilteredCountAsync( string? term,string? status,string? type)
        {
            var query = BaseQuery();

            if (!string.IsNullOrWhiteSpace(term))
                query = query.Where(a =>
                    a.Name.Contains(term) ||
                    a.College.Name.Contains(term) ||
                    a.Location.Name.Contains(term));

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(a => a.Status == status);

            if (!string.IsNullOrWhiteSpace(type))
                query = query.Where(a => a.Type == type);

            return await query.CountAsync();
        }
        public async Task AddAsync(Asset asset)
        {
            asset.CreatedAt = DateTime.UtcNow;
           await _db.Assets.AddAsync(asset);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Asset asset)
        {
           _db.Assets.Update(asset);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var asset = await _db.Assets.FindAsync(id)
                ?? throw new KeyNotFoundException($"Asset with id {id} was not found.");
           _db.Assets.Remove(asset);
           await _db.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id) => await
            _db.Assets.AnyAsync(a => a.Id == id);

        public async Task<Asset?> GetByIdAsync(int id) => 
            await _db.Assets
                    .Include(a => a.College)
            .Include(a => a.Location)
            .Include(a => a.MaintenanceRecords)
            .FirstOrDefaultAsync(a => a.Id == id);






        public async Task<List<Asset>> SearchByCollegeAsync(
    int? collegeId, string? term, string? status, string? type)
        {
            var query = BaseQuery()
                .Where(a => a.CollegeId == collegeId);

            if (!string.IsNullOrWhiteSpace(term))
                query = query.Where(a =>
                    a.Name.Contains(term) ||
                    a.Location.Name.Contains(term));

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(a => a.Status == status);

            if (!string.IsNullOrWhiteSpace(type))
                query = query.Where(a => a.Type == type);

            return await query.OrderBy(a => a.Name).ToListAsync();
        }


        public async Task<List<Asset>> SearchByCollegeAsync(
            int? collegeId, string? term, string? status, string? type,
            int page, int pageSize)
        {
            var query = BaseQuery()
                .Where(a => a.CollegeId == collegeId);

            if (!string.IsNullOrWhiteSpace(term))
                query = query.Where(a =>
                    a.Name.Contains(term) ||
                    a.Location.Name.Contains(term));

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(a => a.Status == status);

            if (!string.IsNullOrWhiteSpace(type))
                query = query.Where(a => a.Type == type);

            return await query
                .OrderBy(a => a.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }   

        public async Task<int> GetFilteredCountByCollegeAsync(
            int? collegeId, string? term, string? status, string? type)
        {
            var query = _db.Assets
                .Where(a => a.CollegeId == collegeId);

            if (!string.IsNullOrWhiteSpace(term))
                query = query.Where(a => a.Name.Contains(term));

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(a => a.Status == status);

            if (!string.IsNullOrWhiteSpace(type))
                query = query.Where(a => a.Type == type);

            return await query.CountAsync();
        }




        public async Task<List<Asset>> GetByCollegeAsync(int collegeId) =>
            await _db.Assets
                .Include(a => a.College)
              
                .Include(a => a.Location)
                .Include(a => a.MaintenanceRecords)
                .Where(a => a.CollegeId == collegeId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();




        public async Task<Asset?> GetByIdForCollegeAsync(int id, int collegeId)=>
            await _db.Assets
                .Include(a => a.College)
                .Include(a => a.Location)
                .Include(a =>a.MaintenanceRecords)
                .FirstOrDefaultAsync(a => a.Id ==id && a.CollegeId==collegeId);


        //edit Asset
        public async Task<Asset?> GetByIdForEditAsync(int id, int collegeId) =>
            await _db.Assets
                .Include(a =>a.Location)
                .FirstOrDefaultAsync(a => a.Id ==id && a.CollegeId ==collegeId);

        public async Task<List<Asset>> GetByLocationAsync(int locationId, int collegeId) =>
            await _db.Assets
            .Include(a => a.Location)
            .Include(a => a.College)
            .Where(a => a.LocationId == locationId && a.CollegeId == collegeId)
            .OrderBy(a => a.Name)
            .ToListAsync();
        
    }
}
