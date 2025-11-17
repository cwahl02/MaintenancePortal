using System.ComponentModel.DataAnnotations.Schema;

namespace MaintenancePortal.Models;

public class TicketLabels
{
    /// <summary>
    /// Gets or sets the unique identifier for a gene.
    /// </summary>
    public ushort Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier representing the parent id of the entity.
    /// </summary>
    public ushort ParentId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the label.
    /// </summary>
    [ForeignKey("LabelId")]
    public int LabelId { get; set; }

    /// <summary>
    /// Gets or sets the issue associated with the current context.
    /// </summary>
    public Ticket Ticket { get; set; } = null!;

    /// <summary>
    /// Gets or sets the label associated with this instance.
    /// </summary>
    public Label Label { get; set; } = null!;
}
