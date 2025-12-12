namespace MaintenancePortal.Models;

/// <summary>
/// Represents a view model containing detailed information for displaying a user's profile.
/// </summary>
public class UserProfileViewModel
{
    public required string Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Username { get; set; }
    public required string DisplayName { get; set; }
    public string? Bio { get; set; }
    public required DateTime Birthdate { get; set; }
    public required DateTime CreatedAt { get; set; }
    public bool CanEdit { get; set; }
}
