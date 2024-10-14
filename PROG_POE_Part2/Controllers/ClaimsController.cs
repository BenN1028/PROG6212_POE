using Microsoft.AspNetCore.Mvc;

public class ClaimsController : Controller
{
    // Simulated database or claims storage
    private List<Claim> claimsDb = new List<Claim>();

    public class ClaimSubmissionViewModel
    {
        public int HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public string AdditionalNotes { get; set; }
    }

    // Change this action to return the SubmitClaim view
    [HttpGet]
    [Route("Home/SubmitClaim")] // This makes it accessible via /Home/SubmitClaim
    public IActionResult SubmitClaim()
    {
        return View();
    }

    [HttpPost]
    [Route("Home/SubmitClaim")] // Same route for POST requests
    public IActionResult SubmitClaim(ClaimSubmissionViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Add the new claim to the database (in-memory list for now)
            var newClaim = new Claim
            {
                ClaimId = claimsDb.Count + 1,
                HoursWorked = model.HoursWorked,
                HourlyRate = model.HourlyRate,
                AdditionalNotes = model.AdditionalNotes,
                Status = "Pending",
                DateSubmitted = DateTime.Now
            };

            claimsDb.Add(newClaim);

            // Redirect to a confirmation or claims list page
            return RedirectToAction("ClaimConfirmation");
        }

        // If model state is invalid, return the form with validation errors
        return View(model);
    }


    public IActionResult ClaimConfirmation()
    {
        return View();
    }
}
