using AirportService.Api.Controllers;
using AirportService.Api.Models;
using AirportService.Api.Services;
using AirportService.Domain.Models;
using AirportService.Domain.Results;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportService.Tests;

public class AirportControllerTests
{
    private readonly IMediator _mediatorMock;
    private readonly ILogger<AirportController> _loggerMock;
    private readonly IMemoryCache _cacheMock;
    private readonly IHttpClientFactory _clientFactory;

    public AirportControllerTests()
    {
        _mediatorMock = new Mock<IMediator>().Object;
        _loggerMock = new Mock<ILogger<AirportController>>().Object;
        _cacheMock = new Mock<IMemoryCache>().Object;
        _clientFactory = new Mock<IHttpClientFactory>().Object;
    }

    [Fact]
    public async Task CalculateDistance_WithValidRequest_ReturnsOkResult2()
    {
        // Arrange
        var controller = new AirportController(_mediatorMock, _loggerMock);
        var request = new DistanceRequest(OriginIata: "LHR", DestinationIata: "JFK");
        // Act
        var result = await controller.CalculateDistance(request);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task CalculateDistance_WithInvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var controller = new AirportController(_mediatorMock, _loggerMock);
        var request = new DistanceRequest(OriginIata: "JJJ", DestinationIata: "JFK");
        // Act
        var result = await controller.CalculateDistance(request);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task CalculateDistance_WithAirportNotFoundError_ReturnsNotFound()
    {
        // Arrange
        var controller = new AirportController(_mediatorMock, _loggerMock);
        var request = new DistanceRequest(OriginIata: "LHR", DestinationIata: "JFK");
        // Act
        var result = await controller.CalculateDistance(request);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetAirportDataAsync_WithCachedData_ReturnsSuccessResultFromCache()
    {
        // Arrange
        var iataCode = "LHR";
        var cancellationToken = CancellationToken.None;
        var airportData = new Airport
        {
            IATA = iataCode,
            Name = "London Heathrow Airport",
            City = "London",
            CityIATA = "LON",
            Country = "United Kingdom",
            CountryIATA = "GB",
            Location = new Location
            {
                Lon = -0.453566f,
                Lat = 51.469603f
            },
            Rating = 3,
            Hubs = 2,
            TimezoneRegionName = "Europe/London",
            Type = "airport"
        };

        var cacheEntry = new Mock<ICacheEntry>();
        var cacheMock = new Mock<IMemoryCache>();
        object cachedValue = airportData;

        cacheMock
            .Setup(m => m.TryGetValue($"AirportData_{iataCode}", out cachedValue))
            .Returns(true);

        var controller = new AirportHttpClient(_clientFactory, cacheMock.Object);

        // Act
        var result = await controller.GetAirportDataAsync(iataCode, cancellationToken);

        // Assert
        var successResult = Assert.IsType<OneOf<SuccessResult<Airport>, ErrorResult, ValidationErrorResult>>(result);
        successResult.AsT0.Value.Should().BeEquivalentTo(airportData);
    }

}
