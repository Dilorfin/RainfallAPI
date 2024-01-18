using Polly.Extensions.Http;
using Polly;

namespace RainfallAPI.Extensions;

public static class HttpClientBuilderExtensions
{
    public static IHttpClientBuilder AddGeneralRetryPolicy(this IHttpClientBuilder builder)
    {
        return builder.AddPolicyHandler(GetRetryPolicy());
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => !msg.IsSuccessStatusCode)
            .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}
