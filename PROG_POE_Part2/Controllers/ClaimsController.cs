using Microsoft.AspNetCore.Mvc;
using MyApp.Models;  // Explicitly using your custom Claim class
using System.Linq;
using System.Collections.Generic;

public class ClaimsController : Controller
{
    // Simulated list of claims (replace with actual database or service calls)
    private List<MyApp.Models.Claim> claimsDb = new List<MyApp.Models.Claim>();

    [HttpGet]
    public IActionResult VerifyClaims()
    {
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
            Status = c.Status  // Include the status for display if needed
        }).ToList();

        return View(model);
    }

    [HttpPost]
    public IActionResult ApproveClaim(int claimId)
    {
        // Find the claim and approve it
        var claim = claimsDb.FirstOrDefault(c => c.ClaimId == claimId);
        if (claim != null)
        {
            claim.Status = "Approved";
        }
        return RedirectToAction("VerifyClaims");
    }

    [HttpPost]
    public IActionResult RejectClaim(int claimId)
    {
        // Find the claim and reject it
        var claim = claimsDb.FirstOrDefault(c => c.ClaimId == claimId);
        if (claim != null)
        {
            claim.Status = "Rejected";
        }
        return RedirectToAction("VerifyClaims");
    }
}
