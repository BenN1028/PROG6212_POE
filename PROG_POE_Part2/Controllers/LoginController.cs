using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

public class LoginController : Controller
{
    // GET: Login
    public IActionResult Index()
    {
        return View("Login"); // This should return the Login view
    }

    // POST: Login
    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Fetch user from database based on login credentials
            var user = GetUserFromDatabase(model.Username, model.Password);

            if (user != null)
            {
                // Set the user's session
                HttpContext.Session.SetString("Username", user.Username);

                // Check the user's role and redirect accordingly
                if (user.Role == "ProgrammeCoordinator" || user.Role == "AcademicManager")
                {
                    return RedirectToAction("VerifyClaims", "Claims");
                }
                else
                {
                    return RedirectToAction("Dashboard", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password."); // Error message for invalid login
            }
        }

        return View("Login", model); // Return the same view with the model if login fails
    }

    // Simulated method to get user details from the database
    private User GetUserFromDatabase(string username, string password)
    {
        var users = new List<User>
        {
            new User { Username = "coordinator1", Password = "password", Role = "ProgrammeCoordinator" },
            new User { Username = "manager1", Password = "password", Role = "AcademicManager" },
            new User { Username = "lecturer1", Password = "password", Role = "Lecturer" }
        };

        return users.FirstOrDefault(u => u.Username == username && u.Password == password);
    }
}
