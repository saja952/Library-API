
namespace Library.Application.Common;

public enum ServiceError
{
    None,
    NotFound,
    Conflict,
    ValidationError
}

public class ServiceResult<T>
{
    public bool IsSuccess { get; init; }

    public T? Data { get; init; }

    public ServiceError Error { get; init; }

    public string? ErrorMessage { get; init; }

    public static ServiceResult<T> Success(T data)
    {
        return new ServiceResult<T>
        {
            IsSuccess = true,
            Data = data,
            Error = ServiceError.None
        };
    }

    public static ServiceResult<T> Failure(
        ServiceError error,
        string message)
    {
        return new ServiceResult<T>
        {
            IsSuccess = false,
            Error = error,
            ErrorMessage = message
        };
    }
}
