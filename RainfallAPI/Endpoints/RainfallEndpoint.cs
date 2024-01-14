using RainfallAPI.Models;

namespace RainfallAPI.Endpoints;

public class RainfallEndpoint : IBaseEndpoint
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<RainfallEndpoint> _logger;
    private readonly Uri _baseRainfallUrl;

    public RainfallEndpoint(IHttpClientFactory httpClientFactory, IConfiguration config, ILogger<RainfallEndpoint> logger)
    {
        _httpClientFactory = httpClientFactory;
        _baseRainfallUrl = new Uri(config.GetValue<string>("Settings:RainfallAPI")!);
        _logger = logger;
    }

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
        _logger.LogDebug($"Requested station: {stationId} with count: {count}");
        if (count < 1 || count > 100)
        {
            return Results.BadRequest(new Error("Count is out of allowed range"));
        }

        using var httpClient = _httpClientFactory.CreateClient();

        var apiResponse = await httpClient.GetAsync(_baseRainfallUrl + $"flood-monitoring/id/stations/{stationId}/readings?_sorted&_limit={count}");

        var responseModel = await apiResponse.Content.ReadFromJsonAsync<ReadingRainfallApiModel>();

        if (!responseModel!.items.Any())
        {
            _logger.LogTrace("No data found for the station");
            return Results.NotFound(new Error("No data found for the station"));
        }

        var result = new Result(responseModel!.items
            .Select(it => new RainfallReading(it.dateTime, it.value)));

        _logger.LogDebug("Result: " + result.ToString());
        return Results.Ok(result);
    }
}
