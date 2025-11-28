using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaintenancePortal.Models;

public class Ticket
{
    [Required, Key]
    public int Id { get; set; }

    [ForeignKey("ParentId")]
    public int? ParentId { get; set; }

    [Required, MaxLength(256)]
    public required string Title { get; set; }

    [Required, MaxLength(2048)]
    public required string Description { get; set; }

    [Required]
    public required bool Status { get; set; }

    [Required]
    [Column(TypeName = "datetime2")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("CreatedByUser")]
    [Required]
    public required string CreatedByUserId { get; set; }


    [Required]
    public required User CreatedByUser { get; set; }

    public ICollection<TicketLabel> IssueLabels { get; set; } = new HashSet<TicketLabel>();
}
