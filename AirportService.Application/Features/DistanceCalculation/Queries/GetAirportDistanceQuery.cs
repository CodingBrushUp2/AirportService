using AirportService.Api.Services;
using AirportService.Domain.Results;
using AirportService.Domain.Services;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using OneOf;
using System;

namespace AirportService.Application.Features.DistanceCalculation.Queries
{
    public record GetAirportDistanceQuery(
        string Origin,
        string Destination)
        : IRequest<OneOf<SuccessResult<double>, ErrorResult, ValidationErrorResult>>;

    public class GetAirportDistanceQueryHandler : IRequestHandler<GetAirportDistanceQuery, OneOf<SuccessResult<double>, ErrorResult, ValidationErrorResult>>
    {
        private readonly AirportHttpClient _airportClient;
        private readonly IMemoryCache _cache;

        public GetAirportDistanceQueryHandler(AirportHttpClient airportClient, IMemoryCache cache)
        {
            _airportClient = airportClient;
            _cache = cache;
        }

        public async Task<OneOf<SuccessResult<double>, ErrorResult, ValidationErrorResult>> Handle(GetAirportDistanceQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"Distance_{request.Origin}_{request.Destination}";
            if (_cache.TryGetValue(cacheKey, out double distance))
            {
                return new SuccessResult<double>(distance);
            }

            var airPort1 = await _airportClient.GetAirportDataAsync(request.Origin, cancellationToken);
            if (airPort1.IsT1 || airPort1.IsT2)
            {
                return new ErrorResult("Origin airport not found!" + airPort1);
            }
            var airPort2 = await _airportClient.GetAirportDataAsync(request.Destination, cancellationToken);
            if (airPort1.IsT1 || airPort1.IsT2)
            {
                return new ErrorResult("Destination airport not found!" + airPort2);
            }

            var result = DistanceCalculator.CalculateDistanceInMiles(airPort1.AsT0.Value.Location, airPort2.AsT0.Value.Location);
            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(30));
            return new SuccessResult<double>(result);
        }
    }
}
