using System.ComponentModel.DataAnnotations;

namespace MaintenancePortal.Models;

public class Label
{
    /// <summary>
    /// Gets or sets the unique identifier for the label.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name associated with the label.
    /// </summary>
    [Required, MaxLength(64)]
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the background color as a hexadecimal color code.
    /// </summary>
    /// <remarks>The value must be a valid hexadecimal color code, either in 3-character or 6-character
    /// format, and must start with a '#' character.</remarks>
    [Required, MaxLength(7)]
    [RegularExpression("^#(?:[A-Fa-f0-9]{3}){1,2}", ErrorMessage = "BackgroundColor must be a valid hex color code.")]
    public required string BackgroundColor { get; set; } // e.g., Hex color code

    /// <summary>
    /// Gets or sets the text color as a hexadecimal color code.
    /// </summary>
    /// <remarks>The value must be a valid hexadecimal color code, either in 3-character or 6-character
    /// format, and must start with a '#' character.</remarks>
    [MaxLength(7)]
    [RegularExpression("^#(?:[A-Fa-f0-9]{3}){1,2}", ErrorMessage = "TextColor must be a valid hex color code.")]
    public string? TextColor { get; set; } // e.g., Hex color code

    /// <summary>
    /// Gets or sets the description associated with the label.
    /// </summary>
    [MaxLength(256)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the user who created the label.
    /// </summary>
    [Required]
    public required string CreatedByUserId { get; set; }

    /// <summary>
    /// Gets or sets the user who created the associated label.
    /// </summary>
    /// <remarks>This is a navigation property.</remarks>
    public required User CreatedByUser { get; set; }

    /// <summary>
    /// Gets or sets the collection of labels associated with the issue.
    /// </summary>
    public ICollection<TicketLabels> TicketLabels { get; set; } = new HashSet<TicketLabels>();
}
