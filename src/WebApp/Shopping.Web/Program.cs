using BuildingBlocks.Logging;
using BuildingBlocks.Polices;
using Polly;
using Polly.Retry;
using Polly.CircuitBreaker;
using Serilog;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Register the LoggingDelegatingHandler as a transient service
builder.Services.AddTransient<LoggingDelegatingHandler>();

// Add Refit HTTP Client Factory
builder.Services.AddRefitClient<ICatalogService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]!);
    })
    .AddHttpMessageHandler<LoggingDelegatingHandler>()
    .AddPolicyHandler((serviceProvider, request) =>
    {
        var logger = serviceProvider.GetRequiredService<ILogger<ICatalogService>>();

        return PolicyFactory.GetHttpRetryPolicy(logger);
    })
    .AddPolicyHandler((serviceProvider, request) =>
    {
        var logger = serviceProvider.GetRequiredService<ILogger<ICatalogService>>();

        return PolicyFactory.GetCircuitBreakerPolicy(logger);
    });

builder.Services.AddRefitClient<IBasketService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]!);
    })
    .AddHttpMessageHandler<LoggingDelegatingHandler>()
    .AddPolicyHandler((serviceProvider, request) =>
    {
        var logger = serviceProvider.GetRequiredService<ILogger<IBasketService>>();

        return PolicyFactory.GetHttpRetryPolicy(logger);
    })
    .AddPolicyHandler((serviceProvider, request) =>
    {
        var logger = serviceProvider.GetRequiredService<ILogger<IBasketService>>();

        return PolicyFactory.GetCircuitBreakerPolicy(logger);
    });

builder.Services.AddRefitClient<IOrderingService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]!);
    })
    .AddHttpMessageHandler<LoggingDelegatingHandler>()
    .AddPolicyHandler((serviceProvider, request) =>
    {
        var logger = serviceProvider.GetRequiredService<ILogger<IOrderingService>>();

        return PolicyFactory.GetHttpRetryPolicy(logger);
    })
    .AddPolicyHandler((serviceProvider, request) =>
    {
        var logger = serviceProvider.GetRequiredService<ILogger<IOrderingService>>();
            
        return PolicyFactory.GetCircuitBreakerPolicy(logger);
    });

// Add Serilog config to Elasticsearch
builder.Host.UseSerilog(SeriLogger.Configure);

// Register Health Checks
builder.Services.AddHealthChecks()
                    .AddUrlGroup(
                        new Uri($"{builder.Configuration["ApiSettings:GatewayAddress"]}/health"),
                        name: "Yarp Gateway Health",
                        failureStatus: HealthStatus.Degraded,
                        tags: new[] { "yarp", "gateway" }
                    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

app.Run();
