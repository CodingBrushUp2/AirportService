using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using AirportService.Domain.Entities;
using MediatR;
using System.Text.Json;

namespace AirportService.Application.Features.DistanceCalculation.Queries;

public class GetAirportDataQuery: IRequest<Airport>
{
    public string IATACode { get; set; }
    public GetAirportDataQuery(string iATACode)
    {
        IATACode = iATACode;
    }
}
public class GetAirportDataQueryHandler : IRequestHandler<GetAirportDataQuery, Airport>
{
    private readonly IHttpClientFactory _httpClientFactory;

    public GetAirportDataQueryHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<Airport> Handle(GetAirportDataQuery request, CancellationToken cancellationToken)
    {
        return await _httpClientFactory.GetAirportByIATACode(request.IATACode);
    }
}
public async Task<Airport> GetAirportData(string iataCode)
{
    var client = _httpClientFactory.CreateClient("AirportClient");
    var response = await client.GetAsync($"{iataCode}");
    response.EnsureSuccessStatusCode();

    var content = await response.Content.ReadAsStringAsync();
    return JsonSerializer.Deserialize<Airport>(content) ?? throw new InvalidOperationException("Unable to deserialize airport data.");
}