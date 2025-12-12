namespace MaintenancePortal.Models;

/// <summary>
/// Represents the data required to edit an existing support ticket.
/// </summary>
public class TicketEditViewModel
{
    public int Id { get; set; }
    public bool IsOpen { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
