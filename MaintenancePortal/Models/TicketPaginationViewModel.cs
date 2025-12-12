namespace MaintenancePortal.Models;

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
