using RainfallAPI.Models;

namespace RainfallAPI.Services;

public class RainfallService(HttpClient httpClient) : IRainfallService
{
    public async Task<ReadingRainfallApiModel?> GetStationsReading(string stationId, int count)
    {
        var endpoint = $"flood-monitoring/id/stations/{stationId}/readings?_limit={count}";
        var apiResponse = await httpClient.GetAsync(endpoint);

        return await apiResponse.Content.ReadFromJsonAsync<ReadingRainfallApiModel>();
    }

    public async Task<ReadingRainfallApiModel?> GetStationsSummary(string stationId, DateTime since)
    {
        string dateTimeString = since.ToString("yyyy-MM-ddTHH:mm:ssZ");
        
        var endpoint = $"flood-monitoring/id/stations/{stationId}/readings?since={dateTimeString}";
        var apiResponse = await httpClient.GetAsync(endpoint);

        return await apiResponse.Content.ReadFromJsonAsync<ReadingRainfallApiModel>();
    }
}