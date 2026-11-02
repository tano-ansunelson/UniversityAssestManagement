using Microsoft.EntityFrameworkCore;
using UniversityAssestManagement.Data;
using UniversityAssetsManagement.Models;

namespace UniversityAssetsManagement.Repositories
{
    public class TechnicianRepository : ITechnicianRepository
    {
        private readonly AppDbContext _db;

        public TechnicianRepository(AppDbContext db)
        {
            _db = db;
        }


        public async Task AddAsync(Technician technician)
        {
          technician.CreateAt = DateTime.Now;
            _db.Technicians.Add(technician);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var technician = await _db.Technicians.FindAsync(id);
            if (technician == null) return false;

            _db.Technicians.Remove(technician);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EmailExistAsync(string email, int? excludeId = null) => 
            await _db.Technicians
                .AnyAsync(t => t.Email.ToLower() == email.ToLower().Trim() && (excludeId ==null || t.Id != excludeId));


        public async Task<bool> ExistsAsync(int id) => 
            await _db.Technicians.AnyAsync(t => t.Id == id);


        public async Task<List<Technician>> GetAllTechniciansAsync() =>

            await _db.Technicians
                .OrderBy(t => t.FullName)
                .ToListAsync();

        public async Task<Technician?> GetTechnicianByIdAsync(int id) =>
            await _db.Technicians.FindAsync(id);


        public async Task UpdateAsync(Technician technician)
        {
        _db.Technicians.Update(technician);
            await _db.SaveChangesAsync();
        }
    }
}
