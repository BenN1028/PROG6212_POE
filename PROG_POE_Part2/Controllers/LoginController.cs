using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

public class LoginController : Controller
{
    // GET: Login
    public ActionResult Index()
    {
        return View();
    }

    // POST: Login
    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Simulate fetching user from database based on login credentials
            var user = GetUserFromDatabase(model.Username, model.Password);

            if (user != null)
            {
                // Simulate setting the user's session or claims
                HttpContext.Session.SetString("Username", user.Username);

                // Check the user's role and redirect accordingly
                if (user.Role == "ProgrammeCoordinator" || user.Role == "AcademicManager")
                {
                    // Redirect to VerifyClaims for coordinators and managers
                    return RedirectToAction("VerifyClaims", "Claims");
                }
                else
                {
                    // Redirect to the dashboard for normal users
                    return RedirectToAction("Dashboard", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
            }
        }

        return View(model);
    }

    // Simulated method to get user details from the database
    private User GetUserFromDatabase(string username, string password)
    {
        // This would typically query your database to find the user by username/password
        // Here is an example with mock data
        var users = new List<User>
        {
            new User { Username = "coordinator1", Password = "password", Role = "ProgrammeCoordinator" },
            new User { Username = "manager1", Password = "password", Role = "AcademicManager" },
            new User { Username = "lecturer1", Password = "password", Role = "Lecturer" }
        };

        return users.FirstOrDefault(u => u.Username == username && u.Password == password);
    }
}
