namespace MaintenancePortal.Models;

/// <summary>
/// Represents a job title within an organization, including its unique identifier and display name.
/// </summary>
public class JobTitle
{
    public int Id { get; set; }
    public string? Name { get; set; }
}
