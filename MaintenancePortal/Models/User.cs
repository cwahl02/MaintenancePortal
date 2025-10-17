using Microsoft.AspNetCore.Identity;

namespace MaintenancePortal.Models;

public class User : IdentityUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required DateTime BirthDate { get; set; }

}
