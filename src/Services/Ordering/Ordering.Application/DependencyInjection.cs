using BuildingBlocks.Behaviors;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Ordering.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                // Register the Validation Behavior
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
                // Register the Loggin Behavior
                config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            });

            return services;
        }
    }
}
