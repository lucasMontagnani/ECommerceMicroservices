using Discount.Grpc;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using BuildingBlocks.Messaging.MassTransit;
using BuildingBlocks.Logging;
using Serilog;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container

// Application Services
builder.Services.AddCarter();

var assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config =>
{
    // Tells the MediatR where to find and register the command and query handler/classes
    config.RegisterServicesFromAssemblies(assembly);
    // Register the Validation Behavior
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    // Register the Loggin Behavior
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

builder.Host.UseSerilog(SeriLogger.Configure);

// Data Services
builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("Database")!);
    // To set/specify the identity field of the entity as username
    opts.Schema.For<ShoppingCart>().Identity(x => x.UserName);
}).UseLightweightSessions();

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.Decorate<IBasketRepository, CacheBasketRepository>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    // options.InstanceName = "Basket";
});

// Grpc Services
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]!);
}) // Configuration to bypass the server (SSL) certificate validation to avoid RpcException
   // (should only be used in dev, can cause security risks in production)
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        return handler;
    });

//Async Communication Services
builder.Services.AddMessageBroker(builder.Configuration);

// Cross-Cutting Services
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

// Register Health Checks
builder.Services.AddHealthChecks()
                    .AddNpgSql(
                        builder.Configuration.GetConnectionString("Database")!,
                        name: "Basket Postgres Health",
                        failureStatus: HealthStatus.Degraded,
                        tags: new[] { "db", "postgres", "basketdb" })
                    .AddRedis(
                        builder.Configuration.GetConnectionString("Redis")!,
                        name: "Redis Health",
                        failureStatus: HealthStatus.Degraded,
                        tags: new[] { "cache", "redis", "basketdb" });

// Register Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
// Configure the HTTP request pipeline

app.MapCarter();

app.UseExceptionHandler(options => { });

app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
