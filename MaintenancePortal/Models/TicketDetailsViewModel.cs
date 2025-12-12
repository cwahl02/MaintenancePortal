namespace MaintenancePortal.Models;

/// <summary>
/// Represents the details of a support ticket for display in the user interface.
/// </summary>
/// <remarks>This view model is typically used to transfer ticket information between the application backend and
/// the presentation layer. It includes properties for ticket metadata, status, and permissions relevant to the current
/// user context.</remarks>
public class TicketDetailsViewModel
{
    public int Id { get; set; }
    public bool IsOpen { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public bool CanEdit { get; set; }
}