using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using UniversityAssetsManagement.Documents;
using UniversityAssetsManagement.Services;

namespace UniversityAssestManagement.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IReportsService _service;

        public ReportsController(IReportsService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var vm = await _service.GetReportsAsync();
            return View(vm);
        }



        public async Task<IActionResult> ExportPdf()
        {
            var model = await _service.GetReportsAsync();

            var document = new ReportsPdfDocument(model);

            var pdfBytes = document.GeneratePdf();

            return File(
                 
                pdfBytes,
                "application/pdf",
                $"AssetReport_{DateTime.Now:yyyy-MM-dd}.pdf"
                );

        }
    }
}
