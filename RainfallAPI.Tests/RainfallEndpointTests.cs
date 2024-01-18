using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using RainfallAPI.Endpoints;
using RainfallAPI.Models;
using RainfallAPI.Services;

namespace RainfallAPI.Tests;

[TestFixture]
public class RainfallEndpointTests
{
    [Test]
    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(101)]
    public async Task GetRainfallReading_CountOutOfRange_ReturnsBadRequest(int count)
    {
        // Arrange
        const string stationId = "";
        var service = CreateRainfallService(null);
        var logger = CreateDefaultLogger();
        
        var endpoint = new RainfallEndpoint(service, logger);

        // Act
        IResult result = await endpoint.GetRainfallReading(stationId, count);

        // Assert
        ClassicAssert.NotNull(result);
        ClassicAssert.IsInstanceOf<BadRequest<Error>>(result);
    }
    
    [Test]
    public async Task GetRainfallReading_NoItemsInResponse_ReturnsNotFound()
    {
        // Arrange
        const string stationId = "";
        var apiReturnModel = new ReadingRainfallApiModel(new List<MeasureRainfallApiModel>());
        var service = CreateRainfallService(apiReturnModel);
        var logger = CreateDefaultLogger();
        
        var endpoint = new RainfallEndpoint(service, logger);

        // Act
        IResult result = await endpoint.GetRainfallReading(stationId);

        // Assert
        ClassicAssert.NotNull(result);
        ClassicAssert.IsInstanceOf<NotFound<Error>>(result);
    }

    [Test]
    public async Task GetRainfallReading_SuccessPath_ReturnsResult()
    {
        // Arrange
        const string stationId = "";
        var apiReturnModel = new ReadingRainfallApiModel(new List<MeasureRainfallApiModel>
        {
            new(DateTime.UtcNow, "", decimal.Zero),
            new(DateTime.UtcNow, "", decimal.One),
            new(DateTime.UtcNow, "", decimal.MaxValue)
        });
        var service = CreateRainfallService(apiReturnModel);
        var logger = CreateDefaultLogger();
        
        var endpoint = new RainfallEndpoint(service, logger);

        // Act
        var result = await endpoint.GetRainfallReading(stationId);

        // Assert
        ClassicAssert.NotNull(result);
        ClassicAssert.IsInstanceOf<Ok<Result>>(result);
        ClassicAssert.AreEqual(apiReturnModel.items.Count(), ((result as Ok<Result>)!).Value!.reading.Count());
    }

    IRainfallService CreateRainfallService(ReadingRainfallApiModel? expected)
    {
        var service = Substitute.For<IRainfallService>();
        service.GetStationsReading(Arg.Any<string>(), Arg.Any<int>())
            .Returns(expected);
        return service;
    }
    
    private static ILogger<RainfallEndpoint> CreateDefaultLogger()
    {
        return Substitute.For<ILogger<RainfallEndpoint>>();
    }
}
