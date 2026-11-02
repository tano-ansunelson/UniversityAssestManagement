using UniversityAssetsManagement.Models;

namespace UniversityAssetsManagement.Repositories
{
    public interface ITechnicianRepository
    {
        Task<List<Technician>> GetAllTechniciansAsync();
        Task<Technician?> GetTechnicianByIdAsync(int id);
        Task AddAsync (Technician technician);

        Task UpdateAsync (Technician technician);
        Task <bool> EmailExistAsync (string email, int ? excludeId = null);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);



    }
}
