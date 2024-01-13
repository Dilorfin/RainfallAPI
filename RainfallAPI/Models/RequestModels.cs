using System.Text.Json.Serialization;

namespace RainfallAPI.Models;

public record MeasureRainfallAPIModel(DateTime dateTime, string measure, decimal value)
{
    [JsonPropertyName("@id")]
    public string id { get; init; }
}

public record ReadingRainfallAPIModel(IEnumerable<MeasureRainfallAPIModel> items);
