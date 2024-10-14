using Microsoft.AspNetCore.Mvc;
using MyApp.Models;

public class ClaimsController : Controller
{
    // Simulated database or claims storage
    private List<LecturerClaim> claimsDb = new List<LecturerClaim>();

    public class LecturerClaimViewModel
    {
        public int HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public string Notes { get; set; }
    }

    [HttpGet]
    public IActionResult SubmitClaim()
    {
        // Display the form to the user
        return View("~/Views/Home/SubmitClaim.cshtml");
    }

    [HttpPost]
    public IActionResult SubmitClaim(LecturerClaimViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Add the new claim to the database (in-memory list for now)
            var newClaim = new LecturerClaim
            {
                ClaimId = claimsDb.Count + 1,
                HoursWorked = model.HoursWorked,
                HourlyRate = model.HourlyRate,
                AdditionalNotes = model.Notes,
                Status = "Pending",
                DateSubmitted = DateTime.Now
            };

            claimsDb.Add(newClaim);

            // Redirect to a confirmation page
            return RedirectToAction("ClaimConfirmation");
        }

        // If the model is not valid, re-display the form with validation errors
        return View("~/Views/Home/SubmitClaim.cshtml", model);
    }

    public IActionResult ClaimConfirmation()
    {
        // Show a confirmation message
        return View("~/Views/Home/ClaimConfirmation.cshtml");
    }
}
