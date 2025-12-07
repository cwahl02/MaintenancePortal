namespace MaintenancePortal.Models;

public class TicketEditableViewModel
{
    public int Id { get; set; }
    public string State { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}