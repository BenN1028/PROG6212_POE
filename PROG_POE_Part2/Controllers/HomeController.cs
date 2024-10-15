using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Login");
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
