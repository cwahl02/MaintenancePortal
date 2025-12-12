namespace MaintenancePortal.Models;

public class TicketEditViewModel
{
    public int Id { get; set; }
    public bool IsOpen { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
