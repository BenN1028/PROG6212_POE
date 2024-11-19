using Microsoft.AspNetCore.Mvc;
using System.Linq;

public class LoginController : Controller
{
    private readonly UserDbContext _context;

    public LoginController(UserDbContext context)
    {
        _context = context;
    }

    // GET: Login
    public IActionResult Index()
    {
        return View("Login");
    }

    // POST: Login
    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Fetch user from database based on login credentials
            var user = _context.Users
                .FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);

            if (user != null)
            {
                // Set the user's session
                HttpContext.Session.SetString("Username", user.Username);

                // Check the user's role and redirect accordingly
                if (user.Role == "ProgrammeCoordinator" || user.Role == "AcademicManager")
                {
                    return RedirectToAction("VerifyClaims", "Claims");
                }
                else if (user.Role == "HR")
                {
                    return RedirectToAction("GenerateReport", "Claims");
                }
                else
                {
                    return RedirectToAction("Dashboard", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password.");
            }
        }

        return View("Login", model);
    }
}
