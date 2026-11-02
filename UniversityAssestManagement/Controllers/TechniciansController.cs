using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityAssetsManagement.Services;
using static UniversityAssetsManagement.Models.ViewModels.TechnicianViewModel;

namespace UniversityAssetsManagement.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class TechniciansController : Controller
    {
        private readonly ITechnicianService _service;

        public TechniciansController(ITechnicianService service)
        {
            _service = service;
        }


        //get/ technicians
        public async Task<IActionResult> Index()
        {
            var vm = await _service.GetAllAsync();
            return View(vm);
        }


        //get/technicians /create
        public IActionResult Create() => View(new SaveTechnicianViewModel());

        //post/technicians/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaveTechnicianViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var (Success, Error) = await _service.CreateAsync(model);
            if (!Success)
            {
                ModelState.AddModelError("Error", Error!);
                return View(model);
            }



            TempData["Success"] = $"Technician \"{model.FullName}\" created successfully!";
            return RedirectToAction(nameof(Index));
        }


        //get /technicians/edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            var vm = await _service.GetForEditAsync(id);
            if (vm == null) return NotFound();
            return View(vm);

        }

        //post /technicians/edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SaveTechnicianViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (Success, Error) = await _service.UpdateAsync(model);
            if (!Success)
            {
              ModelState.AddModelError("Error", Error!);
                return View(model);
            }
            TempData["Success"] = $"\"{model.FullName}\" updated successfully.";
            return RedirectToAction(nameof(Index));
        }



        //post /technicians/toggleactive/{id}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                await _service.ToggleActiveAsync(id);
                TempData["Success"] = "Technician status updated successfully.";
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "Technician not found.";
            }
            return RedirectToAction(nameof(Index));
        }






    }
}
