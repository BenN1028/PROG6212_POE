using Microsoft.AspNetCore.Mvc;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using ClosedXML;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;

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

    public async Task<IActionResult> ViewUsers()
    {
        var users = await _context.Users
            .Select(u => new User
            {
                Id = u.Id,
                Username = u.Username,
                Role = u.Role,
                ContactInfo = u.ContactInfo
            })
            .ToListAsync();

        return View(users);
    }

    // Action to update user details
    [HttpPost]
    public IActionResult UpdateUser(int Id, string Username, string Role, string ContactInfo)
    {
        // Find the user in the database by Id
        var user = _context.Users.FirstOrDefault(u => u.Id == Id); // Updated to use Id

        if (user != null)
        {
            // Update the user fields with the new values
            user.Username = Username;
            user.Role = Role;
            user.ContactInfo = ContactInfo;

            // Save changes to the database
            _context.SaveChanges();
        }

        // Redirect to the ViewUsers page to reflect the updated data
        return RedirectToAction("ViewUsers");
    }
}