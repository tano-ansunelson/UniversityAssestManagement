using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityAssestManagement.Data;
using UniversityAssestManagement.Models;

namespace UniversityAssetsManagement.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly AppDbContext _db;
        public LocationRepository(AppDbContext db) => _db = db;

        public async Task AddAsync(Location location)
        {
            _db.Locations.AddAsync(location);
            await _db.SaveChangesAsync();


        }

        public async Task<List<College>> GetAllGroupedAsync() =>
            await _db.Colleges
                .Include(c => c.Locations)
                    .ThenInclude(l => l.Assets)
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();

        public async Task<Location?> GetByIdAsync(int id) => await _db.Locations.FindAsync(id);





        public async Task<List<Location>> GetByCollegeIdAsync(int collegeId) =>
          await _db.Locations
              .Include(l => l.Assets)
              .Where(l => l.CollegeId == collegeId)
              .OrderBy(l => l.Name)
              .ToListAsync();

        public async Task<bool> ExistsAsync(int collegeId, string name) =>
                await _db.Locations
                    .AnyAsync(l => l.CollegeId == collegeId &&
                    l.Name.ToLower() == name.ToLower().Trim());

        public async Task UpdateAsync(Location location)
        {
            _db.Locations.Update(location);
            await _db.SaveChangesAsync();
        }
    }

}

  
