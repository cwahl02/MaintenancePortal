using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaintenancePortal.Models;

/// <summary>
/// Represents a support or issue tracking ticket, including details about its status, description, and audit
/// information.
/// </summary>
/// <remarks>A Ticket contains information about a reported issue or request, including its creation and
/// modification timestamps, status, and the user who created it. This class is typically used in help desk, customer
/// support, or issue tracking systems to manage and track the lifecycle of individual tickets.</remarks>
public class Ticket
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the item is open.
    /// </summary>
    [Required]
    public bool IsOpen { get; set; }

    /// <summary>
    /// Gets or sets the title of the item.
    /// </summary>
    [Required, StringLength(255)]
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets the description for the entity.
    /// </summary>
    /// <remarks>The description is required and must not exceed 1,024 characters in length.</remarks>
    [Required, StringLength(1024)]
    public required string Description { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// </summary>
    [Required, Column(TypeName = "datetime2")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the item was closed.
    /// </summary>
    [Column(TypeName = "datetime2")]
    public DateTime? ClosedAt{ get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was last modified.
    /// </summary>
    [Required, Column(TypeName = "datetime2")]
    public DateTime LastModifiedAt { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the user who created the entity.
    /// </summary>
    [Required]
    public required string CreatedById { get; set; }
}
