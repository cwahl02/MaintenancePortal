using System.ComponentModel.DataAnnotations;

namespace MaintenancePortal.Models;

/// <summary>
/// Represents the data required to register a new user account.
/// </summary>
/// <remarks>This view model is typically used to collect user input during the registration process in a web
/// application. It includes properties for personal information and credentials, and may be used with data validation
/// attributes to enforce input requirements.</remarks>
public class RegisterViewModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime Birthdate { get; set; }
    public string Username { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required, DataType(DataType.Password)]
    public string Password { get; set; }

    [Required, DataType(DataType.Password), Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}
