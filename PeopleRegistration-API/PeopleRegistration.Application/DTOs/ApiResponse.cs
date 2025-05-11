namespace PeopleRegistration.Application.DTOs;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public IEnumerable<string>? Errors { get; set; }
    public T? Data { get; set; }

    public static ApiResponse<T> Ok(T data, string? message = null)
        => new() { Success = true, Data = data, Message = message };

    public static ApiResponse<T> Fail(IEnumerable<string> errors, string? message = null)
        => new() { Success = false, Errors = errors, Message = message };
}
public class EmptyClass { }
