using RainfallAPI.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

HttpClient httpClient = new()
{
    BaseAddress = new Uri(builder.Configuration["Settings:RainfallAPI"]),
};

app.MapGet("/rainfall/id/{stationId}/readings", async (string stationId, int count = 10) =>
{
    if (count < 1 && count > 100)
    {
        return Results.BadRequest(new Error("Count is out of allowed range", null));
    }

    try
    {
        var apiResponse = await httpClient.GetAsync($"flood-monitoring/id/stations/{stationId}/readings?_sorted&_limit={count}");
        if (apiResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return Results.NotFound(new Error("Count is out of allowed range", null));
        }

        var responceModel = await apiResponse.Content.ReadFromJsonAsync<ReadingRainfallAPIModel>();

        var result = new Result(responceModel.items
            .Select(it => new RainfallReading(it.dateTime, it.value)));

        return Results.Ok(result);
    }
    catch (Exception ex) when (ex.InnerException != null)
    {
        var error = new Error(ex.Message, new List<ErrorDetail>()
        {
            new ErrorDetail("InnerException", ex.InnerException.Message)
        });
        return Results.Json(error, statusCode: StatusCodes.Status500InternalServerError);
    }
    catch (Exception ex)
    {
        return Results.Json(new Error(ex.Message, null), 
            statusCode: StatusCodes.Status500InternalServerError);
    }
});

app.Run("http://localhost:3000");
