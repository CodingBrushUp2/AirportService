namespace AirportService.Domain.Results;

public record NotFoundResult(IEnumerable<string> Errors)
{
    public override string ToString()
    {
        return $"NotFoundResult: {string.Join(", ", Errors)}";
    }
}