using Microsoft.AspNetCore.Mvc;
using UniversityAssetsManagement.Services;

namespace UniversityAssestManagement.Controllers
{
    public class LocationsController : Controller
    {
        private readonly ILocationService _service;

        public LocationsController(ILocationService service)=> _service = service;
        public async Task<IActionResult> Index()
        {
          var vm = await _service.GetAllGroupedAsync();

            return View(vm);
        }
    }
}
