using FluentValidation;

namespace AirportService.Api.Models;

public class DistanceRequestValidator : AbstractValidator<DistanceRequest>
{
    public DistanceRequestValidator()
    {
        RuleFor(x => x.OriginIata)
            .Length(3)
            .Matches("^[A-Z]+$").WithMessage("Origin IATA code must be 3 uppercase letters.");

        RuleFor(x => x.DestinationIata)
            .Length(3)
            .Matches("^[A-Z]+$").WithMessage("Destination IATA code must be 3 uppercase letters.");
    }
}