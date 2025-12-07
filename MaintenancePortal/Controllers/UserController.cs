using MaintenancePortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MaintenancePortal.Controllers;

public class UserController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public UserController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }


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

        if(string.IsNullOrEmpty(model.EmailOrUsername) || string.IsNullOrEmpty(model.Password))
        {
            ModelState.AddModelError(string.Empty, "Email/Username and Password are required.");
            return View(model);
        }

        string? username = null;

        if(model.EmailOrUsername.Contains("@"))
        {
            User? userByEmail = await _userManager.FindByEmailAsync(model.EmailOrUsername);
            if(userByEmail != null)
            {
                username = userByEmail.UserName;
            }
        }
        else
        {
            User? userByUsername = await _userManager.FindByNameAsync(model.EmailOrUsername);
            if(userByUsername != null)
            {
                username = userByUsername.UserName;
            }
        }

        if(username == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or username.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(username, model.Password, model.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Ticket");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            User user = new User
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Birthdate = model.Birthdate
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Login", "User");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }
}
