namespace Sc3Hosted.Shared.Helpers;
public class ServiceResponse<T>
{
    public ServiceResponse(T data, string message, bool success = true)
    {
        Data = data;
        Message = message;
        Success = success;
    }
    public ServiceResponse(string message, bool success = false)
    {
        Message = message;
        Success = success;
    }

    public T Data { get; set; } = default!;
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; } = true;
}

public class ServiceResponse
{
    public ServiceResponse(string message, bool success = false)
    {
        Message = message;
        Success = success;
    }

    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }
}
