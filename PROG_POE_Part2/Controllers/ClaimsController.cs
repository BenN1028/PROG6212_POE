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
                DocumentPath = c.SupportingDocument,
                Status = c.Status,
                RejectionReason = c.RejectionReason
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

        [HttpPost]
        public IActionResult RejectClaim(int claimId, string rejectionReason)
        {
            var claim = claimsDb.FirstOrDefault(c => c.ClaimId == claimId);
            if (claim != null)
            {
                claim.Status = "Rejected";
                claim.RejectionReason = rejectionReason; // Set the rejection reason
            }
            return RedirectToAction("VerifyClaims");
        }

        [HttpGet]
        public IActionResult MyClaims()
        {
            var lecturerName = User.FindFirst(ClaimTypes.Name)?.Value;
            var lecturerName1 = lecturerName; ;
            var myClaims = claimsDb.Where(c => c.LecturerName == lecturerName).ToList();

            var model = myClaims.Select(c => new ClaimViewModel
            {
                ClaimId = c.ClaimId,
                HoursWorked = c.HoursWorked,
                HourlyRate = c.HourlyRate,
                Status = c.Status,
                DocumentPath = c.SupportingDocument,
                RejectionReason = c.RejectionReason
            }).ToList();

            return View(model);
        }
    }
}