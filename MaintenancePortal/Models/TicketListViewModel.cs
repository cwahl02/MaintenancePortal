namespace MaintenancePortal.Models;

public class TicketListViewModel
{
    public string Query;
    public PaginationMetadata PaginationMetadata { get; set; } = new PaginationMetadata();
    public IEnumerable<int> Pagination { get; set; } = new List<int>();
    public IEnumerable<TicketIndexViewModel> Tickets { get; set; } = new List<TicketIndexViewModel>();
}
