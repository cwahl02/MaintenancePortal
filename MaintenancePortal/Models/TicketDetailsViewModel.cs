namespace MaintenancePortal.Models;

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