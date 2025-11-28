namespace MaintenancePortal.Models;

public class Permission
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    public int? ParentId { get; set; }
    public Permission? Parent { get; set; }

    public ICollection<Permission>? Children { get; set; }
}
