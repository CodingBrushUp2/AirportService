using AirportService.Api.Models;
using AirportService.Application.Features.DistanceCalculation.Queries;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AirportService.Api.Controllers;
/// <summary>
/// AirportController
/// </summary>
/// <param name="mediator"></param>
/// <param name="logger"></param>
[ApiController]
[Route("[controller]")]
public class AirportController(IMediator mediator, ILogger<AirportController> logger) : ControllerBase
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly ILogger<AirportController> _logger = logger;

    /// <summary>
    /// CalculateDistance between two airports using their IATA codes (e.g. LHR, AMS, etc.)
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("CalculateDistance")]
    public async Task<IActionResult> CalculateDistance([FromBody] DistanceRequest request)
    {
        _logger.LogInformation($"Received request to calculate distance between {request.OriginIata} and {request.DestinationIata}.");

        DistanceRequestValidator validator = new();
        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning($"Invalid request: {validationResult}");
            return BadRequest(new ApiResponse<IEnumerable<ValidationFailure>>
            {
                StatusCode = 400,
                Message = "Invalid request",
                Data = validationResult.Errors
            });
        }

        var query = new GetAirportDistanceQuery(request.OriginIata, request.DestinationIata);
        var distanceResult = await _mediator.Send(query);

        return distanceResult.Match<IActionResult>(
            success =>
            {
                string resultValue = success is not null ? $"{success.Value} miles" : "unknown";
                _logger.LogInformation($"Distance between {request.OriginIata} and {request.DestinationIata} is {resultValue}");
                return Ok(new ApiResponse
                {
                    StatusCode = 200,
                    Message = $"Distance between {request.OriginIata} and {request.DestinationIata} is {resultValue}",
                    Data = resultValue
                });
            },
            error =>
            {
                _logger.LogWarning($"One or more airports not found.");
                return NotFound(new ApiResponse
                {
                    StatusCode = 404,
                    Message = "One or more airports not found.",
                    Data = error.ErrorMessage,
                });
            },
            validationError =>
            {
                _logger.LogWarning($"Invalid request: {validationError.Errors}");
                return BadRequest(new ApiResponse
                {
                    StatusCode = 400,
                    Message = "Invalid request",
                    Errors = validationError.Errors
                });
            }
        );
    }
}
