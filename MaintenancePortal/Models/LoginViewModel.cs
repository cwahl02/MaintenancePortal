using System.ComponentModel.DataAnnotations;

namespace MaintenancePortal.Models;

public class LoginViewModel
{
    [Required, Display(Name = "Username or Email")]
    public string PrimaryIdentifier { get; set; }
    [Required, DataType(DataType.Password)]
    public string Password { get; set; }
    public bool RememberMe { get; set; }
}
