using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaintenancePortal.Models;

public enum TicketState
{
    Open,
    InProgress,
    Closed
}

public class Ticket
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(255)]
    public required string Title { get; set; }
    [Required, StringLength(1024)]
    public required string Description { get; set; }

    [Required, Column(TypeName = "datetime2")]
    public DateTime CreatedAt { get; set; }
    [Required, Column(TypeName = "nvarchar(32)")]
    public TicketState State { get; set; }
    [Required]
    public required string CreatedById { get; set; }
}
