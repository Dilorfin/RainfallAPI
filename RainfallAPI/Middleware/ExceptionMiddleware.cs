using RainfallAPI.Models;

namespace RainfallAPI.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex) when (ex.InnerException != null)
        {
            _logger.LogError(ex.Message, ex);
            await HandleExceptionWithInternalAsync(httpContext, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var error = new Error(exception.Message);

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(error);
    }

    private async Task HandleExceptionWithInternalAsync(HttpContext context, Exception exception)
    {
        var error = new Error(exception.Message, new List<ErrorDetail>()
        {
            new ErrorDetail("InnerException", exception.InnerException!.Message)
        });

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(error);
    }
}
