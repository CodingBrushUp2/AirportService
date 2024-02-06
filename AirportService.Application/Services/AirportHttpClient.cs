#nullable disable
using AirportService.Application.Interfaces;
using AirportService.Domain.Models;
using AirportService.Domain.Results;
using Microsoft.Extensions.Caching.Memory;
using OneOf;
using System.Text.Json;

namespace AirportService.Api.Services;

public class AirportHttpClient : IAirportHttpClient
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

    public AirportHttpClient(IHttpClientFactory clientFactory, IMemoryCache cache)
    {
        _clientFactory = clientFactory;
        _cache = cache;
    }

    public async Task<OneOf<SuccessResult<Airport>, ErrorResult, ValidationErrorResult>> GetAirportDataAsync(string iataCode, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(iataCode, nameof(iataCode));

        if (_cache.TryGetValue($"AirportData_{iataCode}", out var cachedAirportData))
        {
            return new SuccessResult<Airport>((Airport)cachedAirportData);
        }

        var client = _clientFactory.CreateClient("AirportClient");
        var response = await client.GetAsync($"airports/{iataCode}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return new ErrorResult($"Error fetching data for {iataCode}. {response.StatusCode}");
        }

        try
        {
            string content = await response.Content.ReadAsStringAsync();

            var airportData = JsonSerializer.Deserialize<Airport>(content);
            if (airportData == null)
            {
                return new ErrorResult($"Could not deserialize the response for IATA Code: {iataCode}");
            }

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(30)); // Set appropriate expiration time
            _cache.Set($"AirportData_{iataCode}", airportData, cacheEntryOptions);

            return new SuccessResult<Airport>(airportData);
        }
        catch (JsonException ex)
        {
            return new ErrorResult($"Error deserializing the response for IATA Code: {iataCode}. {ex.Message}");
        }
    }
}
