using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityAssestManagement.Data;
using UniversityAssestManagement.Models;

namespace UniversityAssestManagement.Repositories
{
    public class CollegeRepository : ICollegeRepository
    {


        private readonly AppDbContext _db;

        public CollegeRepository(AppDbContext db)
        {
            _db = db;
        }

        private IQueryable<College> BaseQuery() => _db.Colleges
            .Include(c => c.Assets)
            .Include(c => c.Locations)
            .AsNoTracking();

        public async Task<List<College>> GetAllAsync()=> 
            await BaseQuery()
            .OrderBy(c => c.Code)
            .ToListAsync();

        public async Task<College?> GetDetailsAsync(int id) =>
            await _db.Colleges
                .Include(c => c.Assets)
                    .ThenInclude(a => a.Location)
                .AsNoTracking()
                .FirstOrDefaultAsync( c => c.Id==id);
            
            
       

        public async Task<bool> ExistingAsync(int id)=>
            await _db.Colleges.AnyAsync(c => c.Id == id);

        public async Task AddAsync(College college)
        {
           _db.Colleges.Add(college);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> CodeExistsAsync(string code) =>
                await _db.Colleges
                    .AnyAsync(c => c.Code.ToLower() == code.ToLower().Trim());

        public async Task DeleteAsync(int id)
        {
           var college = await _db.Colleges.FindAsync(id)
                ??throw new NotImplementedException($"College with id {id}was not found");

            _db.Colleges.Remove(college);
            await _db.SaveChangesAsync();
        }




        //public async Task DeleteAsync(int id)
        //{
        //    var asset = await _db.Assets.FindAsync(id)
        //        ?? throw new KeyNotFoundException($"Asset with id {id} was not found.");
        //    _db.Assets.Remove(asset);
        //    await _db.SaveChangesAsync();
        //}
    }
}
