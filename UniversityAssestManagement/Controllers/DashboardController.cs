using Microsoft.AspNetCore.Mvc;
using UniversityAssetsManagement.Services;


namespace UniversityAssestManagement.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _service;

        public DashboardController(IDashboardService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var vm = await _service.GetDashboardAsync();

            return View(vm);
        }

     
    }
}
