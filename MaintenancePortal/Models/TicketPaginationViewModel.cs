namespace MaintenancePortal.Models;

/// <summary>
/// Represents a paginated view model for displaying a list of tickets, including ticket data and pagination details.
/// </summary>
/// <remarks>This view model is typically used to present ticket lists in a paginated format, such as in web
/// applications or dashboards. It provides information about the current page, total pages, and counts of open and
/// closed tickets to support navigation and filtering scenarios.</remarks>
public class TicketPaginationViewModel
{
    public List<TicketIndexViewModel> Tickets { get; set; }
    public bool? TicketState { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalOpenTickets { get; set; }
    public int TotalClosedTickets { get; set; }
    public int TotalTickets { get; set; }
}
