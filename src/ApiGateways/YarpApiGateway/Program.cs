// Add services to the container.
using BuildingBlocks.Logging;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
    {
        options.Window = TimeSpan.FromSeconds(10);
        options.PermitLimit = 5;
    });
});

builder.Host.UseSerilog(SeriLogger.Configure);

// Configure the HTTP request pipeline.
var app = builder.Build();

app.UseRateLimiter();

app.MapReverseProxy();

app.Run();
