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
