using Microsoft.AspNetCore.Mvc;
using UniversityAssestManagement.Models.ViewModels;
using UniversityAssestManagement.Services;

namespace UniversityAssestManagement.Controllers
{
    public class CollegeController : Controller
    {

        private readonly ICollegeService _service;

        public CollegeController(ICollegeService service)
        {
            _service = service;
        }
        //Get college
        public async Task<IActionResult> Index()
        {

           var vm = await _service.GetAllAsync();

            return View(vm);
        }

        //get college/details/ 1,2,3,

        public async Task<IActionResult> Details (int id)
        {

            var vm = await _service.GetDetailsAsync(id);
            if (vm is null) return NotFound();

            return View(vm);
        }



        //get/ college/ add college

        public IActionResult AddCollege()=> View(new AddCollegeViewModel());




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCollege(AddCollegeViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            //check duplicate code
            if (await _service.CodeExistsAsync(model.Code))
            {

                ModelState.AddModelError("Code",
                    $"A college with code \"{model.Code.ToUpper()}\" already exists."
                    );
                return View(model);
            }


            await _service.AddAsync(model);
            TempData["Success"] = $"College \"{model.Name}\" added Successfully";
            return RedirectToAction(nameof(Index));
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    //var college =
        //        await _service.DeleteAsync(id);
        //    TempData["Success"] = $"\"{college.name}\" deleted.";
        //}


        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{

        //    var asset = await _service.GetByIdAsync(id);
        //    if (asset is null) return NotFound();

        //    await _service.DeleteAsync(id);
        //    TempData["Success"] = $"\"{asset.Name}\" deleted.";
        //    return RedirectToAction(nameof(Index));


       // }

    }
}
