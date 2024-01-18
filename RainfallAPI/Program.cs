using Microsoft.OpenApi.Models;
using MinimalApi.Endpoint.Extensions;
using RainfallAPI.Extensions;
using RainfallAPI.Middleware;
using RainfallAPI.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IRainfallService, RainfallService>();
builder.Services.AddHttpClient<IRainfallService, RainfallService>(client =>
    {
        client.Timeout = TimeSpan.Parse(builder.Configuration["Settings:Timeout"]!);
        client.BaseAddress = new Uri(builder.Configuration["Settings:RainfallAPI"]!);
    }).AddGeneralRetryPolicy();

builder.Services.AddEndpoints();
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
