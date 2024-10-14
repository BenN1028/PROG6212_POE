using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    // Sample user store for demonstration purposes
    private List<User> userDb = new List<User>
    {
        new User { Username = "user1", Password = "password1" },
        new User { Username = "user2", Password = "password2" }
    };

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Validate user credentials
            var user = userDb.FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);

            if (user != null)
            {
                // User found and password matches
                HttpContext.Session.SetString("Username", user.Username); // Store user info in session
                return RedirectToAction("Dashboard");
            }
            else
            {
                // User not found or password does not match
                ModelState.AddModelError(string.Empty, "Incorrect username or password.");
            }
        }

        return View(model);
    }

    public IActionResult Dashboard()
    {
        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Remove("Username"); // Clear the session
        return RedirectToAction("Index"); // Redirect to the welcome page
    }

}
