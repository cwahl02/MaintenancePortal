using System.ComponentModel.DataAnnotations;

namespace MaintenancePortal.Models;

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
