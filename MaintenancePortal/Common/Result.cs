namespace MaintenancePortal.Common;

public record Result(
    bool Status,
    string? Message = null,
    Exception? Exception = null)
{
    public static Result Success(string? message = null) => new(true, message);
    public static Result Failure(string message, Exception? exception = null) => new(false, message, exception);
}

public record Result<T>(
    bool Status,
    T? Value = default,
    string? Message = null,
    Exception? Exception = null)
{
    public static Result<T> Success(T value, string? message = null) => new(true, value, message);
    public static Result<T> Failure(string message, Exception? exception = null) => new(false, default, message, exception);
}
