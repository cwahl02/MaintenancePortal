namespace MaintenancePortal.Models;

public class ServiceResult<TData>
{
    public bool Status { get; set; }
    public string? Message { get; set; }
    public TData? Data { get; set; }

    public ServiceResult(bool status, string? message, TData? data)
    {
        Status = status;
        Message = message;
        Data = data;
    }

    // Factory method for success result
    public static ServiceResult<TData> Success(TData? data = default)
    {
        return new ServiceResult<TData>(true, null, data);
    }

    // Factory method for failure result
    public static ServiceResult<TData> Fail(string message, TData? data = default)
    {
        return new ServiceResult<TData>(false, message, data);
    }
}