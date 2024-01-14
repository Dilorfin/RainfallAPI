using System.Text.Json.Serialization;

namespace RainfallAPI.Models;

public record MeasureRainfallApiModel(DateTime dateTime, string measure, decimal value)
{
    [JsonPropertyName("@id")]
    public string id { get; init; }
}

public record ReadingRainfallApiModel(IEnumerable<MeasureRainfallApiModel> items);
