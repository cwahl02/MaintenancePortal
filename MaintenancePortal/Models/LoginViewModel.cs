using System.ComponentModel.DataAnnotations;

namespace MaintenancePortal.Models;

/// <summary>
/// Represents the data required for a user to log in using a username or email address, password, and an optional
/// 'remember me' setting.
/// </summary>
public class LoginViewModel
{
    [Required, Display(Name = "Username or Email")]
    public string EmailOrUsername { get; set; }

    [Required, DataType(DataType.Password)]
    public string Password { get; set; }
    public bool RememberMe { get; set; }
}
