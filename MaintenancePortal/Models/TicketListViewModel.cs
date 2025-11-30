namespace MaintenancePortal.Models;

public class TicketListViewModel
{
    public string Query { get; set; }
    public string Status { get; set; }
    public string Sort { get; set; }
    public int Page { get; set; }
    public int TotalPages { get; set; }

    public IEnumerable<Ticket> Tickets { get; set; }
}
