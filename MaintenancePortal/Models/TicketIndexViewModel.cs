namespace MaintenancePortal.Models;

/// <summary>
/// Represents a view model containing summary information for a displaying a ticket on index list.
/// </summary>
/// <remarks>This model is typically used to display ticket details in lists or overviews, providing key
/// information such as status, creation date, and user details. It does not include the full ticket history or related
/// entities.</remarks>
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
