using Microsoft.AspNetCore.Mvc;
using PROG_POE_Part2.Models;  // Ensure you have your models in this namespace
using System.Collections.Generic;
using System.Linq;

public class ClaimsController : Controller
{
    // Simulated list of claims
    private static List<Claim> claimsDb = new List<Claim>();

    // This method serves the form to submit a claim
    [HttpGet]
    public IActionResult SubmitClaim()
    {
        return View(new LecturerClaimViewModel());
    }

    public IActionResult ClaimConfirmation()
    {
        return View();
    }


    // This method processes the claim submission
    [HttpPost]
    public IActionResult SubmitClaim(LecturerClaimViewModel model, IFormFile supportingDocument)
    {
        if (ModelState.IsValid)
        {
            // Check if a file was uploaded
            if (supportingDocument != null)
            {
                // Validate file size (e.g., max 2 MB)
                if (supportingDocument.Length > 2 * 1024 * 1024) // 2 MB
                {
                    ModelState.AddModelError("SupportingDocument", "File size must be less than 2 MB.");
                    return View(model);
                }

                // Validate file type
                var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
                var fileExtension = Path.GetExtension(supportingDocument.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("SupportingDocument", "Only PDF, DOCX, and XLSX files are allowed.");
                    return View(model);
                }

                // Define the uploads directory
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                // Ensure the uploads directory exists
                if (!Directory.Exists(uploadsDir))
                {
                    Directory.CreateDirectory(uploadsDir); // Create the directory if it does not exist
                }

                // Store the file securely
                var fileName = Path.GetFileNameWithoutExtension(supportingDocument.FileName);
                var newFileName = $"{fileName}_{Guid.NewGuid()}{fileExtension}"; // Ensure unique file name
                var filePath = Path.Combine(uploadsDir, newFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    supportingDocument.CopyTo(stream);
                }

                // Create a new Claim object and add it to the claimsDb list
                var newClaim = new Claim
                {
                    ClaimId = claimsDb.Count + 1, // Generate a new ID
                    LecturerName = "Lecturer Name", // You can replace this with the actual logged-in user's name
                    HoursWorked = model.HoursWorked,
                    HourlyRate = model.HourlyRate,
                    DateSubmitted = DateTime.Now,
                    Notes = model.Notes,
                    SupportingDocument = newFileName, // Store the document name
                    Status = "Pending" // Default status for new claims
                };

                claimsDb.Add(newClaim); // Add to the list
                ViewBag.UploadedFileName = newFileName; // Show uploaded file name

                return RedirectToAction("ClaimConfirmation");
            }
        }

        return View(model);
    }



    // This method fetches and displays pending claims
    [HttpGet]
    public IActionResult VerifyClaims()
    {
        // Restrict access based on user role
        if (User.IsInRole("ProgrammeCoordinator") || User.IsInRole("AcademicManager"))
        {
            return View(); // Return 403 Forbidden if the user is not authorized
        }

        // Fetch all pending claims
        var pendingClaims = claimsDb.Where(c => c.Status == "Pending").ToList();

        var model = pendingClaims.Select(c => new ClaimViewModel
        {
            ClaimId = c.ClaimId,
            LecturerName = c.LecturerName,
            HoursWorked = c.HoursWorked,
            HourlyRate = c.HourlyRate,
            DateSubmitted = c.DateSubmitted,
            Notes = c.Notes,
            Status = c.Status
        }).ToList();

        return View(model);
    }


    // POST method for approving claims
    [HttpPost]
    public IActionResult ApproveClaim(int claimId)
    {
        var claim = claimsDb.FirstOrDefault(c => c.ClaimId == claimId);
        if (claim != null)
        {
            claim.Status = "Approved";
        }
        return RedirectToAction("VerifyClaims");
    }

    // POST method for rejecting claims
    [HttpPost]
    public IActionResult RejectClaim(int claimId)
    {
        var claim = claimsDb.FirstOrDefault(c => c.ClaimId == claimId);
        if (claim != null)
        {
            claim.Status = "Rejected";
        }
        return RedirectToAction("VerifyClaims");
    }
}
