using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaintenancePortal.Models;

public class TicketViewModel
{
    public int? Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }

    public DateTime? Timestamp { get; set; }
    public TicketState? State { get; set; }
    public string? CreatedById { get; set; }
}
