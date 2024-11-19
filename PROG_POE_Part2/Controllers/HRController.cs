using Microsoft.AspNetCore.Mvc;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using ClosedXML;
using ClosedXML.Excel;

public class HRController : Controller
{
    private readonly UserDbContext _context;

    public HRController(UserDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult ExportToExcel()
    {
        try
        {
            // Fetch approved claims
            var approvedClaims = _context.Claims
                .Where(c => c.Status == "Approved")
                .Select(c => new
                {
                    c.ClaimId,
                    c.LecturerName,
                    c.HoursWorked,
                    c.HourlyRate,
                    TotalAmount = c.HoursWorked * c.HourlyRate,
                    c.DateSubmitted,
                    c.Status,
                    c.Notes
                })
                .ToList();

            // Generate the Excel file
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Approved Claims");
                var currentRow = 1;

                // Add headers
                worksheet.Cell(currentRow, 1).Value = "Claim ID";
                worksheet.Cell(currentRow, 2).Value = "Lecturer Name";
                worksheet.Cell(currentRow, 3).Value = "Hours Worked";
                worksheet.Cell(currentRow, 4).Value = "Hourly Rate";
                worksheet.Cell(currentRow, 5).Value = "Total Amount";
                worksheet.Cell(currentRow, 6).Value = "Date Submitted";
                worksheet.Cell(currentRow, 7).Value = "Status";
                worksheet.Cell(currentRow, 8).Value = "Notes";

                // Add data
                foreach (var claim in approvedClaims)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = claim.ClaimId;
                    worksheet.Cell(currentRow, 2).Value = claim.LecturerName;
                    worksheet.Cell(currentRow, 3).Value = claim.HoursWorked;
                    worksheet.Cell(currentRow, 4).Value = claim.HourlyRate;
                    worksheet.Cell(currentRow, 5).Value = claim.TotalAmount;
                    worksheet.Cell(currentRow, 6).Value = claim.DateSubmitted.ToString("dd MMM yyyy");
                    worksheet.Cell(currentRow, 7).Value = claim.Status;
                    worksheet.Cell(currentRow, 8).Value = claim.Notes;
                }

                // Save to memory stream
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ApprovedClaims.xlsx");
                }
            }
        }
        catch (Exception ex)
        {
            // Log the error (adjust logging as needed)
            return BadRequest($"An error occurred: {ex.Message}");
        }
    }
}