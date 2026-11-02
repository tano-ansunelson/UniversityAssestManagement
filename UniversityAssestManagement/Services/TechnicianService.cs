using Microsoft.Extensions.Logging.Abstractions;
using UniversityAssetsManagement.Models;
using UniversityAssetsManagement.Models.ViewModels;
using UniversityAssetsManagement.Repositories;
using static UniversityAssetsManagement.Models.ViewModels.TechnicianViewModel;

namespace UniversityAssetsManagement.Services
{
    public class TechnicianService : ITechnicianService
    {
        private readonly ITechnicianRepository _repo;

        public TechnicianService(ITechnicianRepository repo)
        {
            _repo = repo;
        }


        public async Task<(bool Success, string? Error)> CreateAsync(SaveTechnicianViewModel model)
        {
           if(await _repo.EmailExistAsync(model.Email))
                return (false , $" A technician with email {model.Email} already exists.");

            var technician = new Technician
            {
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Specialty = model.Speciality,
                IsActive = true,
            };
            await _repo.AddAsync(technician);
            return (true, null);
        }

        public async Task<TechnicianListViewModel> GetAllAsync()
        {
          var technicians = await _repo.GetAllTechniciansAsync();

            return new TechnicianListViewModel
            {
                TotalActive = technicians.Count(t => t.IsActive),
                TotalInactive = technicians.Count(t => !t.IsActive),


                Technicians = technicians.Select(t => new TechnicianRow
                {
                    Id = t.Id,
                    FullName = t.FullName,
                    Email = t.Email,
                    PhoneNumber = t.PhoneNumber,
                    Speciality = t.Specialty,
                    IsActive = t.IsActive,
                    CreateAt = t.CreateAt,
                    JobsAssigned = t.MaintenanceRecords.Count

                }).ToList()
            };
        }

        public async Task<SaveTechnicianViewModel?> GetForEditAsync(int id)
        {
            var tech =await _repo.GetTechnicianByIdAsync(id);
            if (tech is null)
                return null;

            return new SaveTechnicianViewModel
            {
                Id = tech.Id,
                FullName = tech.FullName,
                Email = tech.Email,
                PhoneNumber = tech.PhoneNumber,
                Speciality = tech.Specialty,
                IsActive = tech.IsActive

            };

                
        }

        public async Task ToggleActiveAsync(int id)
        {
           var tech = await _repo.GetTechnicianByIdAsync(id) 
                ?? throw new KeyNotFoundException($"Technician with ID {id} not found.");

            tech.IsActive = !tech.IsActive;
            await _repo.UpdateAsync(tech);
        }

        public async Task<(bool Success, string? Error)> UpdateAsync(SaveTechnicianViewModel model)
        {

            var tech = await _repo.GetTechnicianByIdAsync(model.Id);
            if (tech is null) return (false, "Technician not found");


            //check if email is being updated and if it already exists

            if(await _repo.EmailExistAsync(model.Email, model.Id))
                return (false, $" A technician with email {model.Email} already exists.");

            tech.FullName = model.FullName.Trim();
            tech.Email = model.Email.Trim().ToLower();
            tech.PhoneNumber = model.PhoneNumber.Trim();
            tech.Specialty = model.Speciality.Trim();
            tech.IsActive = model.IsActive;

            await _repo.UpdateAsync(tech);
            return (true, null);


        }
    }
}
