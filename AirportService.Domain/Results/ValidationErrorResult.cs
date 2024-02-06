namespace AirportService.Domain.Results;

public record ValidationErrorResult(IEnumerable<string> Errors)
{
    public override string ToString() => string.Join(", ", Errors);
}