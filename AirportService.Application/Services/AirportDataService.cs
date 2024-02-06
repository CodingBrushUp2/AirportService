using AirportService.Domain.Models;
using AirportService.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using OneOf;
using AirportService.Domain.Results;

namespace AirportService.Application.Services;
public class AirportDataService : IAirportDataService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<AirportDataService> _logger;

    public AirportDataService(IHttpClientFactory clientFactory, ILogger<AirportDataService> logger)
    {
        _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<OneOf<SuccessResult<Airport>, ErrorResult, ValidationErrorResult>> GetAirportDataAsync(string iataCode)
    {
        if (string.IsNullOrWhiteSpace(iataCode))
        {
            throw new ArgumentException("IATA code cannot be null or whitespace.", nameof(iataCode));
        }

        try
        {
            var client = _clientFactory.CreateClient("AirportClient");
            var response = await client.GetAsync($"airports/{iataCode}");

            if (!response.IsSuccessStatusCode)
            {
                string message = $"Failed to fetch data for IATA code {iataCode}. Status code: {response.StatusCode}";
                _logger.LogError(message);
                return new ErrorResult(message);
            }

            var content = await response.Content.ReadAsStringAsync();
            var airport = JsonSerializer.Deserialize<Airport>(content);

            if (airport == null)
            {
                return new ValidationErrorResult(new List<string> { $"Could not deserialize the response for IATA Code: {iataCode}" });
            }

            return new SuccessResult<Airport>(airport);
        }
        catch (Exception ex)
        {
            string message = $"An error occurred while fetching data for IATA code {iataCode}. " + ex;
            _logger.LogError(message);
            return new ErrorResult(message);
        }
    }
}
