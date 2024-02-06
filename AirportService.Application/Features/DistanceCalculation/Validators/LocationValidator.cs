using AirportService.Domain.Models;
using FluentValidation;

namespace AirportService.Application.Features.DistanceCalculation.Validators;

public class LocationValidator : AbstractValidator<Location>
{
    public LocationValidator()
    {
        RuleFor(x => x.Lat).InclusiveBetween(-90f, 90f); // Latitude range
        RuleFor(x => x.Lon).InclusiveBetween(-180f, 180f); // Longitude range
    }
}