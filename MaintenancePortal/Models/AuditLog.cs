using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaintenancePortal.Models;

public class AuditLog
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the entity. This value is required.
    /// </summary>
    [Required]
    public required string EntityName { get; set; } // e.g., "Issue", "User"

    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    [Required]
    public required string EntityId { get; set; } // e.g., "A1B2-C3D4", "4", "user123"

    /// <summary>
    /// Gets or sets the action to be performed. Common values include "Create", "Update", and "Delete".
    /// </summary>
    [Required]
    public required string Action { get; set; } // e.g., "Create", "Update", "Delete"

    /// <summary>
    /// Gets or sets the timestamp indicating when the associated event occurred.
    /// </summary>
    [Required]
    public required DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who performed the action.
    /// </summary>
    /// <remarks>This is a foreign key referencing the entity which generated the log, either user or a sub system.</remarks>
    [Required]
    public required string ActorId { get; set; }

    /// <summary>
    /// Gets or sets the user who performed the action.
    /// </summary>
    [ForeignKey(nameof(ActorId))]
    public User? Actor { get; set; }

    /// <summary>
    /// Gets or sets the metadata associated with the object.
    /// </summary>
    public string? Metadata { get; set; } // e.g., JSON string with additional details
}
