using MaintenancePortal.Models;
using Microsoft.AspNetCore.Mvc;

namespace MaintenancePortal.Controllers;

public class UserController : Controller
{
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if(!ModelState.IsValid)
        {
            return View(model);
        }

        // TODO: Login manager
        var result = true;

        if (result)
        {
            return RedirectToAction("Index", "Ticket");
        }

        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = true;

        if (result)
        {
            return RedirectToAction("Login", "User");
        }

        return View();
    }
}
