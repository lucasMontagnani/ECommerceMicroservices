using Ordering.API;
using Ordering.Application;
using Ordering.Infrastructure;

// Add services to the container
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices();

// Configure the HTTP request pipeline
var app = builder.Build();

app.Run();
