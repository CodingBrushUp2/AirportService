namespace AirportService.Api.Extensions;

public class Success<T>(T value)
{
    public T Value { get; set; } = value;
}

public class NotFound(string message)
{
    public string Message { get; set; } = message;
}

public class ValidationError(IEnumerable<string> errors)
{
    public IEnumerable<string> Errors { get; set; } = errors;
}