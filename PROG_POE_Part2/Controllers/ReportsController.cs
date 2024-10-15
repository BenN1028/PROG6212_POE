using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;

public class ReportsController : Controller
{
    [HttpGet]
    public IActionResult Report()
    {
        var model = new ReportViewModel
        {
            ReportOptions = new SelectList(new[] { "Monthly Claim Summary", "Unpaid Invoices", "Approved Claims", "Pending Claims" })
        };
        return View("~/Views/Home/Report.cshtml", model);
    }

    [HttpPost]
    public IActionResult Generate(ReportViewModel model)
    {
        // Handle report generation logic here
        return View("GeneratedReport", model);
    }

    [HttpPost]
    public IActionResult Export(ReportViewModel model, string format)
    {
        // Handle report export logic here (PDF/Excel)
        return View("ExportedReport", model);
    }
}
