using Microsoft.AspNetCore.Identity;

namespace MaintenancePortal.Models;

public class User : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? Birthdate { get; set; }
    public DateTime? CreatedAt { get; set; }
}
