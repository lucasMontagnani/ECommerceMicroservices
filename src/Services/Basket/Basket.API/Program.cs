var builder = WebApplication.CreateBuilder(args);
// Add services to the container

builder.Services.AddCarter();

var assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config =>
{
    // Tells the MediatR where to find and register the command and query handler/classes
    config.RegisterServicesFromAssemblies(assembly);
    // Register the Validation Behavior
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    // Register the Loggin Behavior
    config.AddOpenBehavior(typeof(LogginBehavior<,>));
});

builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("Database")!);
    // To set/specify the identity field of the entity as username
    opts.Schema.For<ShoppingCart>().Identity(x => x.UserName);
}).UseLightweightSessions();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddScoped<IBasketRepository, BasketRepository>();

var app = builder.Build();
// Configure the HTTP request pipeline

app.MapCarter();

app.UseExceptionHandler(options => { });

app.Run();
