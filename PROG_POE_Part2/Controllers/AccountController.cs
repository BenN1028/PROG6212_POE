using Microsoft.AspNetCore.Mvc;

public class LoginController : Controller
{
    // GET: Login
    public ActionResult Index()
    {
        return View();
    }

    // POST: Login
    [HttpPost]
    public ActionResult Index(string username, string password)
    {
        if (IsValidUser(username, password))
        {
            // User is valid, redirect to the dashboard or any other page
            return RedirectToAction("Index", "Dashboard");
        }
        else
        {
            // Invalid user, return an error message
            ViewBag.Message = "Invalid username or password";
            return View();
        }
    }

    // Simulated user validation logic
    private bool IsValidUser(string username, string password)
    {
        // This is where you would check against your database
        // For now, this is just hardcoded logic for testing purposes.
        if (username == "admin" && password == "password")
        {
            return true;
        }
        return false;
    }
}
