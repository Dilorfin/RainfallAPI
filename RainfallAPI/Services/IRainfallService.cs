using RainfallAPI.Models;

namespace RainfallAPI.Services;

public interface IRainfallService
{
    Task<ReadingRainfallApiModel?> GetStationsReading(string stationId, int count);
}