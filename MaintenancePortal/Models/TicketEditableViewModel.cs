namespace MaintenancePortal.Models;

public class TicketEditableViewModel
{
    public int Id { get; set; }
    public TicketState State { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public bool CanEdit { get; set; }
}