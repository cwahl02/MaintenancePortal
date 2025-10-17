using System.ComponentModel.DataAnnotations.Schema;

namespace MaintenancePortal.Models;

public class Issue
{
    public required long Gene { get; set; }
    public required short Depth { get; set; }
    public required short Order { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required DateTime CreatedAt { get; set; }
    [ForeignKey("User")]
    public required string CreatedByUserId { get; set; }

    [NotMapped]
    public string DisplayId => $"{Gene:D8}-{Depth:D3}-{Order:D3}";
}
