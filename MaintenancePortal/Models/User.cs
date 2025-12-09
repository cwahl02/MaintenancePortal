using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaintenancePortal.Models;

public class User : IdentityUser
{
    [Required, StringLength(64)]
    public required string FirstName { get; set; }

    [Required, StringLength(64)]
    public required string LastName { get; set; }

    [Required, Column(TypeName = "date")]
    public DateTime Birthdate { get; set; }

    [Required, Column(TypeName = "datetime2")]
    public DateTime CreatedAt { get; set; }
}
