using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaintenancePortal.Models;

public class Ticket
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    [Required, Key]
    public ushort Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the parent entity.
    /// </summary>
    public ushort? ParentId { get; set; }

    /// <summary>
    /// Gets or sets the title of the entity.
    /// </summary>
    [Required, MaxLength(256)]
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets the description of the entity.
    /// </summary>
    [Required, MaxLength(2048)]
    public required string Description { get; set; }

    /// <summary>
    /// Gets or sets a value indicating the current status of the operation.
    /// </summary>
    [Required]
    public required bool Status { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// </summary>
    [Required]
    [Column(TypeName = "datetime2")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the user who created the entity.
    /// </summary>
    [ForeignKey("CreatedByUser")]
    [Required]
    public required string CreatedById { get; set; }

    /// <summary>
    /// Gets or sets the user who created the associated entity.
    /// </summary>
    /// <remarks>This is a navigation property.</remarks>
    [Required]
    public required User CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the collection of labels associated with the issue.
    /// </summary>
    public ICollection<TicketLabels> TicketLabels { get; set; } = new HashSet<TicketLabels>();
}
