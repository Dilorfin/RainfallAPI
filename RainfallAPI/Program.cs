using Microsoft.OpenApi.Models;
using MinimalApi.Endpoint.Extensions;
using RainfallAPI.Endpoints;
using RainfallAPI.Extentions;
using RainfallAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpoints();
builder.Services.AddHttpClient<IBaseEndpoint, RainfallEndpoint>()
    .AddGeneralRetryPolicy();
builder.Services.AddLogging();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Rainfall Api", Version = "1.0" });
    c.EnableAnnotations();
});

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.MapEndpoints();

app.Run("http://localhost:3000");
