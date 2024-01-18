using RainfallAPI.Models;

namespace RainfallAPI.Services;

public class RainfallService(IHttpClientFactory httpClientFactory, IConfiguration config)
    : IRainfallService
{
    private readonly Uri _baseRainfallUrl = new(config.GetValue<string>("Settings:RainfallAPI")!);
    
    public async Task<ReadingRainfallApiModel?> GetStationsReading(string stationId, int count)
    {
        using var httpClient = httpClientFactory.CreateClient();

        var requestUri = new Uri(_baseRainfallUrl, $"flood-monitoring/id/stations/{stationId}/readings?_limit={count}");
        var apiResponse = await httpClient.GetAsync(requestUri);

        return await apiResponse.Content.ReadFromJsonAsync<ReadingRainfallApiModel>();
    }
}