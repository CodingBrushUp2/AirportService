namespace AirportService.Domain.Results;

public record ErrorResult(string ErrorMessage)
{
    public string ErrorMessage { get; private set; } = ErrorMessage;
}
