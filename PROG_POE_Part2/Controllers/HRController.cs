using Microsoft.AspNetCore.Mvc;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

public class HRController : Controller
{
    // POST: Export to PDF
    [HttpPost]
    public IActionResult ExportToPDF()
    {
        try
        {
            // Path to the Crystal Report file (.rpt)
            string reportPath = Path.Combine(Directory.GetCurrentDirectory(), "Reports", "ApprovedClaimsReport.rpt");

            // Load the Crystal Report
            ReportDocument reportDocument = new ReportDocument();
            reportDocument.Load(reportPath);

            // Set the data source for the report (fetch approved claims)
            var approvedClaims = GetApprovedClaims(); // Replace with your actual method
            reportDocument.SetDataSource(approvedClaims);

            // Export the report to PDF format
            Stream reportStream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

            // Return the PDF as a file download
            return File(reportStream, "application/pdf", "ApprovedClaimsReport.pdf");
        }
        catch (Exception ex)
        {
            // Handle exceptions and return error message
            return BadRequest($"An error occurred while generating the report: {ex.Message}");
        }
    }

    // Mock data source for demonstration (replace with actual database query)
    private List<Claim> GetApprovedClaims()
    {
        // Replace with actual database query to fetch approved claims
        return new List<Claim>
        {
            new Claim { LecturerName = "John Doe", HoursWorked = 15, HourlyRate = 40, Status = "Approved" },
            new Claim { LecturerName = "Jane Smith", HoursWorked = 18, HourlyRate = 45, Status = "Approved" }
        };
    }
}
