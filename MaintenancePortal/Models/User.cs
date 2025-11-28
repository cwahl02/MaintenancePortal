using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaintenancePortal.Models;

public class User : IdentityUser
{
    public int Level { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? BirthDate { get; set; }

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
            var age = today.Year - BirthDate!.Value.Year;
            if (BirthDate.Value.Date > today.AddYears(-age)) age--;
            return age;
        }
    }

    public ICollection<Ticket> CreatedIssues { get; set; } = new HashSet<Ticket>();
    public ICollection<Label> CreatedLabels { get; set; } = new HashSet<Label>();
}
