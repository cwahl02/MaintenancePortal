using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaintenancePortal.Models;

/// <summary>
/// Represents an application user with extended profile information, including personal and account details.
/// </summary>
/// <remarks>In addition to the standard identity properties provided by <see cref="IdentityUser"/>, this class
/// includes fields for first and last name, display name, biography, birthdate, and account creation date. Use this
/// type to access or manage user-specific data within the application.</remarks>
public class User : IdentityUser
{
    [Required, StringLength(64)]
    public required string FirstName { get; set; }

    [Required, StringLength(64)]
    public required string LastName { get; set; }

    [Required, StringLength(64)]
    public required string DisplayName { get; set; }

    [StringLength(256)]
    public string? Bio { get; set; }

    [Required, Column(TypeName = "date")]
    public DateTime Birthdate { get; set; }

    [Required, Column(TypeName = "datetime2")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
