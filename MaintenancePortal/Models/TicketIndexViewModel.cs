namespace MaintenancePortal.Models;

public class TicketIndexViewModel
{
    public required int Id { get; set; }
    public required bool IsOpen { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string UserId { get; set; }
    public required string Username { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime? ClosedAt { get; set; }
    public DateTime DisplayDate
    {
        get
        {
            return IsOpen ? CreatedAt : ClosedAt ?? CreatedAt;
        }

    }
}
