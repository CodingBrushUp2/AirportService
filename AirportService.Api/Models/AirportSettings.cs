namespace AirportService.Api.Models
{
    public class AirportSettings
    {
        public string DefaultUnit { get; set; } = "miles";
        public string AirportApiUrl { get; set; } = string.Empty;
    }
}
