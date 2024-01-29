using System.Globalization;
using RainfallAPI.Models;
using RainfallAPI.Services;

namespace RainfallAPI.Endpoints;

public class RainfallEndpoint(IRainfallService rainfallService, ILogger<RainfallEndpoint> logger)
    : IBaseEndpoint
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/rainfall/id/{stationId}/readings", GetRainfallReading)
            .WithTags("Rainfall")
            .WithDescription("Operations relating to rainfall")
            .Produces<Result>(StatusCodes.Status200OK)
            .Produces<Error>(StatusCodes.Status400BadRequest)
            .Produces<Error>(StatusCodes.Status404NotFound)
            .Produces<Error>(StatusCodes.Status500InternalServerError);

        app.MapGet("/rainfall/{stationId}/readings/summary", GetRainfallSummary);
    }

    public async Task<IResult> GetRainfallSummary(string stationId, int hours = 24)
    {
        if (hours < 1 || hours > 72)
        {
            return Results.BadRequest(new Error("Hours is out of allowed range"));
        }
        
        DateTime dataTime = DateTime.UtcNow.AddHours(-hours);
        
        var responseModel = await rainfallService.GetStationsReading(stationId, hours);
        
        if (responseModel is null || !responseModel.items.Any())
        {
            logger.LogTrace("No data found for the station");
            return Results.NotFound(new Error("No data found for the station"));
        }

        var count = responseModel.items.Count();
        var result = new RainfallSummary(stationId,
            dataTime.ToString("yyyy-MM-ddTHH:mm:ss:ffffzzz"),
            count,
            responseModel.items.Min(e => e.value),
            responseModel.items.Max(e => e.value),
            responseModel.items.Sum(e => e.value) / count);

        logger.LogDebug("Result: " + result);
        return Results.Ok(result);
    }
    
    public async Task<IResult> GetRainfallReading(string stationId, int count = 10)
    {
        logger.LogDebug($"Requested station: {stationId} with count: {count}");
        if (count < 1 || count > 100)
        {
            return Results.BadRequest(new Error("Count is out of allowed range"));
        }

        var responseModel = await rainfallService.GetStationsReading(stationId, count);

        if (responseModel is null || !responseModel.items.Any())
        {
            logger.LogTrace("No data found for the station");
            return Results.NotFound(new Error("No data found for the station"));
        }

        var result = new Result(responseModel!.items
            .Select(it => new RainfallReading(it.dateTime, it.value)));

        logger.LogDebug("Result: " + result);
        return Results.Ok(result);
    }
}
