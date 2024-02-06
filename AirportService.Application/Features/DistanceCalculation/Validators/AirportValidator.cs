using AirportService.Domain.Models;
using FluentValidation;

namespace AirportService.Application.Features.DistanceCalculation.Validators;

public class AirportValidator : AbstractValidator<Airport>
{
    public AirportValidator()
    {
        RuleFor(x => x.IATA).NotEmpty()
            .Length(3)
            .Matches("^[A-Z]+$").WithMessage("Origin IATA code must be 3 uppercase letters.");

        //RuleFor(x => x.Name).NotEmpty();
        //RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.CityIATA).NotEmpty()
            .Length(3)
            .Matches("^[A-Z]+$").WithMessage("Origin IATA code must be 3 uppercase letters.");

        //RuleFor(x => x.Country).NotEmpty();
        RuleFor(x => x.CountryIATA).NotEmpty()
            .Length(3)
            .Matches("^[A-Z]+$").WithMessage("Origin IATA code must be 3 uppercase letters.");

        RuleFor(x => x.Location).NotNull()
            .SetValidator(new LocationValidator());
    }
}
