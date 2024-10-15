using Microsoft.AspNetCore.Mvc;
using MyApp.Models;  // Ensure you have your models in this namespace
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
    public IActionResult SubmitClaim(LecturerClaimViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Create a new Claim object and add it to the claimsDb list
            var newClaim = new Claim
            {
                ClaimId = claimsDb.Count + 1, // Generate a new ID
                LecturerName = "Lecturer Name", // Replace with actual logged-in user's name
                HoursWorked = model.HoursWorked,
                HourlyRate = model.HourlyRate,
                DateSubmitted = DateTime.Now,
                Notes = model.Notes,
                Status = "Pending" // Default status for new claims
            };

            claimsDb.Add(newClaim); // Add to the list

            return RedirectToAction("ClaimConfirmation");
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
