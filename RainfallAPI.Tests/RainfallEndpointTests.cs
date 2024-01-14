using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using RainfallAPI.Endpoints;
using RainfallAPI.Tests.Helpers;
using System.Net;
using Newtonsoft.Json;
using RainfallAPI.Models;

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
        var httpClientFactory = CreateHttpClientFactory("", HttpStatusCode.OK);
        var configuration = CreateDefaultConfiguration();
        var logger = CreateDefaultLogger();
        
        var endpoint = new RainfallEndpoint(httpClientFactory, configuration, logger);

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
        var httpClientFactory = CreateHttpClientFactory(JsonConvert.SerializeObject(apiReturnModel), HttpStatusCode.OK);
        var configuration = CreateDefaultConfiguration();
        var logger = CreateDefaultLogger();
        
        var endpoint = new RainfallEndpoint(httpClientFactory, configuration, logger);

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
        var httpClientFactory = CreateHttpClientFactory(JsonConvert.SerializeObject(apiReturnModel), HttpStatusCode.OK);
        var configuration = CreateDefaultConfiguration();
        var logger = CreateDefaultLogger();
        
        var endpoint = new RainfallEndpoint(httpClientFactory, configuration, logger);

        // Act
        IResult result = await endpoint.GetRainfallReading(stationId);

        // Assert
        ClassicAssert.NotNull(result);
        ClassicAssert.IsInstanceOf<Ok<Result>>(result);
        ClassicAssert.AreEqual(apiReturnModel.items.Count(), ((result as Ok<Result>)!).Value!.reading.Count());
    }
    
    private static IConfiguration CreateDefaultConfiguration()
    {
        var inMemorySettings = new Dictionary<string, string> {
            { "Settings:RainfallAPI", "http://example.com" },
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();
        return configuration;
    }
    private static ILogger<RainfallEndpoint> CreateDefaultLogger()
    {
        return Substitute.For<ILogger<RainfallEndpoint>>();
    }
    private static IHttpClientFactory CreateHttpClientFactory(string response, HttpStatusCode statusCode)
    {
        var messageHandler = new MockHttpMessageHandler(response, statusCode);
        HttpClient httpClient = new HttpClient(messageHandler);

        IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory.CreateClient()
            .Returns(httpClient);
        return httpClientFactory;
    }
}
