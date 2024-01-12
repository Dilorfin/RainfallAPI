using System.Text.Json.Serialization;

namespace RainfallAPI.Models;

record MeasureRainfallAPIModel(DateTime dateTime, string measure, decimal value)
{
    [JsonPropertyName("@id")]
    public string id { get; init; }
}
record ReadingRainfallAPIModel(IEnumerable<MeasureRainfallAPIModel> items);
