using System.ComponentModel.DataAnnotations.Schema;

namespace MaintenancePortal.Models;

public class TicketLabel
{
    public int TicketId { get; set; }

    [ForeignKey("LabelId")]
    public int LabelId { get; set; }

    public Ticket Ticket { get; set; } = null!;
    public Label Label { get; set; } = null!;
}
