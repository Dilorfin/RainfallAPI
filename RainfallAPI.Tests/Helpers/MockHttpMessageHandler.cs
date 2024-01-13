using System.Net;

namespace RainfallAPI.Tests.Helpers;

public class MockHttpMessageHandler(string response, HttpStatusCode statusCode) 
    : HttpMessageHandler
{
    private readonly string _response = response;
    private readonly HttpStatusCode _statusCode = statusCode;

    public string Input { get; private set; }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request.Content != null)
        {
            Input = await request.Content.ReadAsStringAsync();
        }
        return new HttpResponseMessage
        {
            StatusCode = _statusCode,
            Content = new StringContent(_response)
        };
    }
}