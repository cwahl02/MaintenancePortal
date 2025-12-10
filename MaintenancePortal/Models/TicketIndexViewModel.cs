namespace MaintenancePortal.Models;

public class TicketIndexViewModel
{
    public required int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string UserId { get; set; }
    public required string Username { get; set; }
    public required TicketState State { get; set; }
    public required DateTime StateDate { get; set; }
}
