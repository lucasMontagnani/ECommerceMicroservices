using BuildingBlocks.Logging;
using BuildingBlocks.Polices;
using Polly;
using Polly.Retry;
using Polly.CircuitBreaker;
using Serilog;

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
        var logger = serviceProvider
            .GetRequiredService<ILogger<IBasketService>>();

        // Explicitly cast the retry policy to IAsyncPolicy<HttpResponseMessage>
        return (IAsyncPolicy<HttpResponseMessage>)PolicyFactory.GetRetryPolicy(logger);
    })
    .AddPolicyHandler((serviceProvider, request) =>
    {
        var logger = serviceProvider
            .GetRequiredService<ILogger<IBasketService>>();

        // Explicitly cast the circuit breaker policy to IAsyncPolicy<HttpResponseMessage>
        return (IAsyncPolicy<HttpResponseMessage>)PolicyFactory.GetCircuitBreakerPolicy(logger);
    });

builder.Services.AddRefitClient<IBasketService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]!);
    })
    .AddHttpMessageHandler<LoggingDelegatingHandler>()
    .AddPolicyHandler((serviceProvider, request) =>
    {
        var logger = serviceProvider
            .GetRequiredService<ILogger<IBasketService>>();

        // Explicitly cast the retry policy to IAsyncPolicy<HttpResponseMessage>
        return (IAsyncPolicy<HttpResponseMessage>)PolicyFactory.GetRetryPolicy(logger);
    })
    .AddPolicyHandler((serviceProvider, request) =>
    {
        var logger = serviceProvider
            .GetRequiredService<ILogger<IBasketService>>();

        // Explicitly cast the circuit breaker policy to IAsyncPolicy<HttpResponseMessage>
        return (IAsyncPolicy<HttpResponseMessage>)PolicyFactory.GetCircuitBreakerPolicy(logger);
    });

builder.Services.AddRefitClient<IOrderingService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]!);
    })
    .AddHttpMessageHandler<LoggingDelegatingHandler>()
    .AddPolicyHandler((serviceProvider, request) =>
    {
        var logger = serviceProvider
            .GetRequiredService<ILogger<IBasketService>>();

        // Explicitly cast the retry policy to IAsyncPolicy<HttpResponseMessage>
        return (IAsyncPolicy<HttpResponseMessage>)PolicyFactory.GetRetryPolicy(logger);
    })
    .AddPolicyHandler((serviceProvider, request) =>
    {
        var logger = serviceProvider
            .GetRequiredService<ILogger<IBasketService>>();

        // Explicitly cast the circuit breaker policy to IAsyncPolicy<HttpResponseMessage>
        return (IAsyncPolicy<HttpResponseMessage>)PolicyFactory.GetCircuitBreakerPolicy(logger);
    });

// Add Serilog config to Elasticsearch
builder.Host.UseSerilog(SeriLogger.Configure);

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

app.Run();
