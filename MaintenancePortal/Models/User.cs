using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaintenancePortal.Models;

public class User : IdentityUser
{
    /// <summary>
    /// Gets or sets the first name of the individual.
    /// </summary>
    [Required, MaxLength(128)]
    public required string FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name of the individual.
    /// </summary>
    [Required, MaxLength(128)]
    public required string LastName { get; set; }

    /// <summary>
    /// Gets or sets the birth date of the individual.
    /// </summary>
    [Required]
    [Column(TypeName = "date")]
    public required DateTime BirthDate { get; set; }

    /// <summary>
    /// Display full name of the user.
    /// </summary>
    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Gets the age of the individual based on their birth date.
    /// </summary>
    [NotMapped]
    public int Age {
        get
        {
            var today = DateTime.Today;
            var age = today.Year - BirthDate.Year;
            if (BirthDate.Date > today.AddYears(-age)) age--;
            return age;
        }
    }

    /// <summary>
    /// Gets or sets the collection of issues created by the user.
    /// </summary>
    public ICollection<Ticket> CreatedIssues { get; set; } = new HashSet<Ticket>();

    /// <summary>
    /// Gets or sets the collection of labels created by the user.
    /// </summary>
    public ICollection<Label> CreatedLabels { get; set; } = new HashSet<Label>();
}
