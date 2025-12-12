using MaintenancePortal.Data;
using MaintenancePortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MaintenancePortal.Controllers;

/// <summary>
/// Provides actions for viewing and editing user profile information within the application.
/// </summary>
/// <remarks>The ProfileController enables authenticated users to view their own or other users' profiles, as well
/// as update their personal profile details. All actions require the user to be authenticated. Profile data is
/// retrieved and updated using the application's database context. This controller is intended to be used as part of
/// the application's user management and profile features.</remarks>
public class ProfileController : Controller
{
    private readonly AppDbContext _context;
    public ProfileController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles HTTP GET requests to display the public profile page for the specified user.
    /// </summary>
    /// <param name="username">The username of the user whose profile is to be displayed. If null or empty, the method returns a 404 Not Found
    /// result.</param>
    /// <returns>An <see cref="IActionResult"/> that renders the user's profile view if the user exists; otherwise, a 404 Not
    /// Found result.</returns>
    [HttpGet]
    [Route("profile/{username}")]
    public async Task<IActionResult> Profile(string? username = null)
    {
        if (String.IsNullOrEmpty(username))
        {
            return NotFound();
        }

        User? user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        if (user == null)
        {
            return NotFound();
        }

        UserProfileViewModel model = new UserProfileViewModel()
        {
            Id = user.Id,
            FirstName = user.FirstName!,
            LastName = user.LastName!,
            Username = user.UserName!,
            DisplayName = user.DisplayName!,
            Bio = user.Bio,
            Birthdate = user.Birthdate,
            CreatedAt = user.CreatedAt,
            CanEdit = user.Id == User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        };

        return View(model);
    }

    /// <summary>
    /// Handles GET requests for editing the current user's profile by displaying the user edit form populated with the
    /// user's existing information.
    /// </summary>
    /// <remarks>This action retrieves the currently authenticated user's information and pre-populates the
    /// edit form. The user must be authenticated to access this action.</remarks>
    /// <returns>A view that displays the user edit form if the user is found; otherwise, a NotFound result.</returns>
    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        User? user = _context.Users.FirstOrDefault(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (user == null)
        {
            return NotFound();
        }

        UserEditViewModel model = new UserEditViewModel()
        {
            Id = user.Id,
            Username = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Birthdate = user.Birthdate,
            DisplayName = user.DisplayName,
            Bio = user.Bio
        };

        return View(model);
    }

    /// <summary>
    /// Handles HTTP POST requests to update the current user's profile information.
    /// </summary>
    /// <remarks>If the specified user does not exist, the method returns a 404 Not Found result. Only fields
    /// provided in the model are updated; other fields remain unchanged.</remarks>
    /// <param name="model">An object containing the updated profile data for the user. Must not be null and must satisfy all model
    /// validation requirements.</param>
    /// <returns>A redirect to the profile page if the update is successful; otherwise, returns the edit view with validation
    /// errors.</returns>
    [HttpPost]
    public async Task<IActionResult> UpdateProfile(UserEditViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Get the current user from the database
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.Id);
            if (user == null)
            {
                return NotFound(); // If the user doesn't exist, return a 404
            }

            // Update user details from the model
            user.FirstName = model.FirstName ?? user.FirstName;
            user.LastName = model.LastName ?? user.LastName;
            user.UserName = model.Username ?? user.UserName;
            user.Birthdate = model.Birthdate ?? user.Birthdate;
            user.DisplayName = model.DisplayName ?? user.DisplayName;
            user.Bio = model.Bio ?? user.Bio;

            // Save changes to the database
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // After updating, redirect to the profile page
            return RedirectToAction("Profile", new { username = user.UserName });
        }

        // If the model is not valid, return to the edit page
        return View(model);
    }
}
