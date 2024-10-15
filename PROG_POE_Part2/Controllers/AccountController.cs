using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    // GET: /Account/AccessDenied
    public IActionResult AccessDenied()
    {
        return View();
    }
}
