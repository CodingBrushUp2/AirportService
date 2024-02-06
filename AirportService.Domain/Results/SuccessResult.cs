namespace AirportService.Domain.Results;

public record SuccessResult<T>(T Value)
{
    public T Value { get; private set; } = Value;
}
