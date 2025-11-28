namespace MaintenancePortal.Models;

public class UserPermission
{
    public string UserId { get; set; }
    public int PermissionId { get; set; }
    public string AcquisitionType { get; set; }
    public string SourceId { get; set; }
    public DateTime AcquiredOn { get; set; }
}
