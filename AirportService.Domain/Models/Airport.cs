using System.Text.Json.Serialization;

namespace AirportService.Domain.Models;

//https://places-dev.cteleport.com/airports/BUD
public class Airport
{
    [JsonPropertyName("iata")]
    public required string IATA { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("city")]
    public string City { get; set; }

    [JsonPropertyName("city_iata")]
    public string CityIATA { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; }

    [JsonPropertyName("country_iata")]
    public string CountryIATA { get; set; }

    [JsonPropertyName("location")]
    public required Location Location { get; set; }

    [JsonPropertyName("rating")]
    public int Rating { get; set; }

    [JsonPropertyName("hubs")]
    public int Hubs { get; set; }

    [JsonPropertyName("timezone_region_name")]
    public string TimezoneRegionName { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }
}

public class Location
{
    [JsonPropertyName("lon")]
    public float Lon { get; set; }
    [JsonPropertyName("lat")]
    public float Lat { get; set; }
}
