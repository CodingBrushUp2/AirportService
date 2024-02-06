namespace AirportService.Domain.Models;

public record IATACode
{
    private readonly string _value;

    public IATACode(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException("IATA code cannot be null or empty.");
        }

        if (value.Length != 3)
        {
            throw new ArgumentException("IATA code must be 3 characters long.");
        }

        if (!value.All(char.IsLetter))
        {
            throw new ArgumentException("IATA code can only contain letters (A-Z).");
        }

        _value = value;
    }

    public override string ToString()
    {
        return _value;
    }
}