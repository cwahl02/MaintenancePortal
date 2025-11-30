namespace MaintenancePortal.Models;

public class TicketModifyViewModel
{
    public int? Id { get; set; }
    public required string Title { get; set; } = string.Empty;
    public required string Description { get; set; } = string.Empty;

    public TicketModifyViewModel(string title, string description)
    {
        Title = title;
        Description = description;
    }

    public TicketModifyViewModel() { }
}
