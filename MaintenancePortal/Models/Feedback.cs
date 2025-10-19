using System.ComponentModel.DataAnnotations;

namespace MaintenancePortal.Models;

public class Feedback
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the message content.
    /// </summary>
    [Required, MaxLength(1024)]
    public required string Message { get; set; }

    /// <summary>
    /// Gets or sets the name of the individual or entity that submitted the item.
    /// </summary>
    [MaxLength(128)]
    public string SubmittedBy { get; set; } = "Anonymous";

    /// <summary>
    /// Gets or sets the date and time when the submission was made.
    /// </summary>
    [Required]
    public DateTime SubmittedAt { get; set;  } = DateTime.UtcNow;
}