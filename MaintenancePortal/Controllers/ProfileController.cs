using MaintenancePortal.Data;
using MaintenancePortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MaintenancePortal.Controllers;

public class ProfileController : Controller
{
    private readonly AppDbContext _context;
    public ProfileController(AppDbContext context)
    {
        _context = context;
    }

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

    [HttpPost]
    [ValidateAntiForgeryToken]
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
