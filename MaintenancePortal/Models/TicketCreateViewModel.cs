namespace MaintenancePortal.Models;

/// <summary>
/// Represents the data required to create a new support ticket.
/// </summary>
public class TicketCreateViewModel
{
    public string Title { get; set; }
    public string Description { get; set; }
}
