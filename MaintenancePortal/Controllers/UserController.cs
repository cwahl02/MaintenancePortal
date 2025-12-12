using MaintenancePortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MaintenancePortal.Controllers;

/// <summary>
/// Provides user account management actions such as login, logout, and registration for the application.
/// </summary>
/// <remarks>This controller handles user authentication and registration workflows using ASP.NET Core Identity.
/// It supports both username and email-based login, and manages user sign-in and sign-out processes. All actions are
/// intended for use in web application scenarios and rely on the configured Identity services.</remarks>
public class UserController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public UserController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    /// <summary>
    /// Displays the login page or redirects authenticated users to the ticket index page.
    /// </summary>
    /// <remarks>If the user is already authenticated, this method redirects to the ticket index page instead
    /// of displaying the login form. The optional return URL is used to redirect the user after a successful
    /// login.</remarks>
    /// <param name="returnUrl">The URL to redirect to after a successful login. Can be null to use the default redirect location.</param>
    /// <returns>A view result that renders the login page, or a redirect result if the user is already authenticated.</returns>
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Ticket");
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    /// <summary>
    /// Handles user login requests by validating credentials and signing in the user if authentication succeeds.
    /// </summary>
    /// <remarks>If the provided credentials are invalid or required fields are missing, the method returns
    /// the login view with appropriate validation errors. The method supports login using either email or
    /// username.</remarks>
    /// <param name="model">The login information submitted by the user, including email or username, password, and remember-me option.
    /// Cannot be null.</param>
    /// <param name="returnUrl">The URL to redirect to after a successful login. If null, the user is redirected to the default page.</param>
    /// <returns>An <see cref="IActionResult"/> that renders the login view on failure or redirects the user upon successful
    /// authentication.</returns>
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

    /// <summary>
    /// Signs out the current user and redirects to the login page.
    /// </summary>
    /// <remarks>This method clears the user's authentication session and removes the authentication cookie.
    /// It should be called to securely log out a user from the application.</remarks>
    /// <returns>A redirect result that navigates the user to the login page after successful logout.</returns>
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        // Sign out from Identity
        await _signInManager.SignOutAsync();

        Response.Cookies.Delete(".AspNetCore.Identity.Application");

        // Redirect to the login page after logout
        return RedirectToAction("Login", "User");
    }

    /// <summary>
    /// Displays the registration page for new users.
    /// </summary>
    /// <remarks>This action is typically accessed via a GET request to present the registration form to the
    /// user. No user data is pre-populated in the form.</remarks>
    /// <returns>An <see cref="IActionResult"/> that renders the registration view with a new <see cref="RegisterViewModel"/>
    /// instance.</returns>
    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    /// <summary>
    /// Handles user registration by creating a new user account with the provided registration details.
    /// </summary>
    /// <remarks>If the username is already taken or registration fails due to validation errors, the method
    /// returns the registration view with appropriate error messages. The user is automatically signed in after
    /// successful registration.</remarks>
    /// <param name="model">The registration information submitted by the user, including username, password, and personal details. Must not
    /// be null.</param>
    /// <returns>A view displaying the registration form with validation errors if registration fails; otherwise, a redirect to
    /// the login page upon successful registration.</returns>
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Check if the username already exists
            var existingUser = await _userManager.FindByNameAsync(model.Username);
            if (existingUser != null)
            {
                // Add a custom error for the Username field
                ModelState.AddModelError("Username", "The username is already taken.");
                return View(model);
            }

            User user = new User
            {
                UserName = model.Username,
                DisplayName = $"{model.FirstName} {model.LastName}",
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
