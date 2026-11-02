using Microsoft.AspNetCore.Mvc;
using UniversityAssestManagement.Services;
using UniversityAssetsManagement.Models.ViewModels;
using UniversityAssetsManagement.Services;

namespace UniversityAssestManagement.Controllers
{
    public class MaintenanceController : Controller
    {
        private readonly IMaintenanceService _service;
        private readonly IEmailService _email;
        public MaintenanceController(IMaintenanceService service, IEmailService email)
        {
            _service = service;
            _email = email;
        }

        public async Task<IActionResult> Index( int page =1)
        {
            var vm = await _service.GetAllAsync();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int  id, string status)
        {
            if (string.IsNullOrEmpty(status)) {

                TempData["Error"] = "Please selete a valid status.";
                return RedirectToAction(nameof(Index));


            }
            try
            {
                await _service.UpdateStatusAsync(id, status);
                TempData["Success"] = $"Record update to \"{status}\" successfully.";
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "Maintenance record not found.";
            }

            return RedirectToAction(nameof(Index));
        }
       

        public async Task<IActionResult> Update(int id)
        {
            var vm = await _service.GetForUpdateAsync(id);

            if (vm is null) return NotFound();

            return View(vm);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update (UpdateMaintenanceViewModel model)
        {

            if (!ModelState.IsValid) return View(model);

            try
            {
                await _service.UpdateMaintenanceAsync(model);
                TempData["Success"] = "Maintenance record updated successfully.";
                return RedirectToAction(nameof(Index));

            } catch (KeyNotFoundException) {

                return NotFound();
            
            }
        }



        //get/maintenance/assign/5
        public async Task<IActionResult> Assign(int id)
        {
            var vm = await _service.GetForAssignmentAsync(id);
            if (vm is null) return NotFound();
            return View(vm);
        }


        //post/maintenance/assign
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(AssignTechnicianViewModel model)
        {
            if(!ModelState.IsValid)
            {
                var refrehed = await _service.GetForAssignmentAsync(model.RecordId);
                if(refrehed is null) return NotFound();
                model.Technicians = refrehed.Technicians;
                model.AssetName = refrehed.AssetName;
                model.CollegeName = refrehed.CollegeName;
                model.LocationName = refrehed.LocationName;
                model.IssueDescription = refrehed.IssueDescription;
                model.TicketId = refrehed.TicketId;
                return View(model);
            }


            var assignedBy = User.Identity?.Name ?? "SuperAdmin";

           //save assignment

            await _service.AssignTechnicianAsync(model.RecordId, model.TechnicianId, assignedBy);



            //get technician details for the email
            var record = await _service.GetForAssignmentAsync(model.RecordId);
            var tech = record?.Technicians
                .FirstOrDefault(t => t.Id == model.TechnicianId);

            if(tech is not null)
            {
                try
                {
                    await _email.SendAssignmentEmailAsync(new AssignmentEmailModel
                    {
                        TechnicianName = tech.FullName,
                        TechnicianEmail = tech.Email,
                        AssetName = model.AssetName,
                        CollegeName = model.CollegeName,
                        LocationName = model.LocationName,
                        IssueDescription = model.IssueDescription,
                        AssignedBy = assignedBy,
                        AssignedAt = DateTime.Now,
                        TicketId = model.TicketId
                    });
                    TempData["Success"] = $"Technician assigned and email sent to {tech.Email}.";
                }
                catch (Exception ex)
                {
                    TempData["Success"] = "Technician assigned successfully.";
                    TempData["Warning"] = $"Email notification failed: {ex.Message}" +
                        "Please notify the tehnician manually.";
                }
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
