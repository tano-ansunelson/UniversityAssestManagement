using Microsoft.AspNetCore.Mvc;
using UniversityAssestManagement.Services;

namespace UniversityAssestManagement.Controllers
{
    public class AssetsController : Controller
    {



        private readonly IAssetService _service;

        public AssetsController(IAssetService service)
        {
            _service = service;
        }
        //get assets

        public async Task< IActionResult> Index( string? term, string? status, string? type, int page =1)
        {
           var vm = await _service.GetAssetsAsync(term, status, type, page);
            return View(vm);
        }


        //asset/details
        public async Task<IActionResult> Details(int id)
        {
            var asset = await _service.GetByIdAsync(id);
            if (asset == null) return NotFound();
            return View(asset);
        }

        //get asset delete

        public async Task<IActionResult> Delete(int id)
        {
            var asset = await _service.GetByIdAsync(id);
            if(asset is null) return NotFound();
            return View(asset);
        }


        //asset delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {

            var asset = await _service.GetByIdAsync(id);
            if( asset is null) return NotFound();

            await _service.DeleteAsync(id);
            TempData["Success"] =$"\"{asset.Name}\" deleted.";
            return RedirectToAction(nameof(Index));
        
        
        }
    }
}
