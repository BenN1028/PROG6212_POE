using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG_POE_Part2.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace PROG_POE_Part2.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly UserDbContext _context;

        public ClaimsController(UserDbContext context)
        {
            _context = context;
        }
        // Simulated list of claims
        public static List<Claim> claimsDb = new List<Claim>();

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

        public string GetProgressBarClass(string status)
        {
            return status switch
            {
                "Pending" => "bg-warning",
                "Approved" => "bg-success",
                "Rejected" => "bg-danger",
                _ => "bg-secondary"
            };
        }

        public int GetProgressPercentage(string status)
        {
            return status switch
            {
                "Pending" => 50,
                "Approved" => 100,
                "Rejected" => 100,
                _ => 0
            };
        }


        // This method processes the claim submission
        [HttpPost]
        public IActionResult SubmitClaim(LecturerClaimViewModel model, IFormFile supportingDocument)
        {
            if (ModelState.IsValid)
            {
                // Get the lecturer's username from the session
                var lecturerName = HttpContext.Session.GetString("Username");

                // Ensure the username is valid (not null or empty)
                if (string.IsNullOrEmpty(lecturerName))
                {
                    ModelState.AddModelError("", "Lecturer is not logged in.");
                    return View(model);
                }

                // Check if a file was uploaded
                if (supportingDocument == null)
                {
                    ModelState.AddModelError("SupportingDocument", "A supporting document is required.");
                    return View(model);
                }

                // Validate file size (e.g., max 2 MB)
                if (supportingDocument.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("SupportingDocument", "File size must be less than 2 MB.");
                    return View(model);
                }

                // Store the file
                var fileName = Path.GetFileNameWithoutExtension(supportingDocument.FileName);
                var fileExtension = Path.GetExtension(supportingDocument.FileName);
                var newFileName = $"{fileName}_{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", newFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    supportingDocument.CopyTo(stream);
                }

                // Create a new Claim object and add it to the database
                var newClaim = new Claim
                {
                    LecturerName = lecturerName,  // Set LecturerName from the session (logged-in user)
                    HoursWorked = model.HoursWorked,
                    HourlyRate = model.HourlyRate,
                    DateSubmitted = DateTime.Now,
                    Notes = model.Notes,
                    SupportingDocument = newFileName,
                    Status = "Pending"
                };

                _context.Claims.Add(newClaim);
                _context.SaveChanges();

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

            // Fetch all pending claims from the database
            var pendingClaims = _context.Claims
                .Where(c => c.Status == "Pending")
                .Select(c => new ClaimViewModel
                {
                    ClaimId = c.ClaimId,
                    LecturerName = c.LecturerName ?? "Unknown",
                    HoursWorked = c.HoursWorked,
                    HourlyRate = c.HourlyRate,
                    DateSubmitted = c.DateSubmitted,
                    Notes = c.Notes ?? "No notes provided",
                    DocumentPath = c.SupportingDocument ?? string.Empty,
                    Status = c.Status,
                    RejectionReason = c.RejectionReason ?? "N/A"
                })
                .ToList();

            return View(pendingClaims);
        }


        // POST method for approving claims
        [HttpPost]
        public IActionResult ApproveClaim(int claimId)
        {
            // Fetch the claim from the database
            var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == claimId);

            if (claim != null)
            {
                // Update claim status
                claim.Status = "Approved";

                // Save changes to the database
                _context.SaveChanges();
            }

            return RedirectToAction("VerifyClaims");
        }

        [HttpPost]
        public IActionResult RejectClaim(int claimId, string rejectionReason)
        {
            var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == claimId);
            if (claim != null)
            {
                claim.Status = "Rejected";

                // Ensure rejectionReason is handled properly in case it's null
                claim.RejectionReason = string.IsNullOrEmpty(rejectionReason) ? null : rejectionReason;

                _context.SaveChanges();
            }

            return RedirectToAction("VerifyClaims");
        }

        [HttpGet]
        public IActionResult MyClaims()
        {
            // Retrieve the username from the session
            var lecturerName = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(lecturerName))
            {
                // Redirect to login if the username is not found
                return RedirectToAction("Login", "Login");
            }

            // Fetch claims for the logged-in user from the database
            var myClaims = _context.Claims
                .Where(c => c.LecturerName != null && c.LecturerName == lecturerName)
                .Select(c => new ClaimViewModel
                {
                    ClaimId = c.ClaimId,
                    LecturerName = c.LecturerName ?? "Unknown",
                    HoursWorked = c.HoursWorked,
                    HourlyRate = c.HourlyRate,
                    DateSubmitted = c.DateSubmitted,
                    Notes = c.Notes ?? "No notes provided",
                    Status = c.Status ?? "Pending",
                    DocumentPath = c.SupportingDocument ?? string.Empty,
                    RejectionReason = c.RejectionReason ?? "Not applicable"
                })
                .ToList();


            // Pass the list to the view
            return View(myClaims);
        }
    }
}