using Newtonsoft.Json;

namespace TrafficGuard.Application.DTOs.SpeedTelemetry;

public class SpeedTelemetryDTO
{
    [JsonProperty("placa")]
    public string LicensePlate { get; set; } = string.Empty;

    [JsonProperty("velocidade")]
    public int Speed { get; set; }

    [JsonProperty("via")]
    public string RoadName { get; set; } = string.Empty;

    [JsonProperty("data_hora")]
    public DateTime Timestamp { get; set; }
}