using Azure.Core.Pipeline;
using BuildingBlocks.Polices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly.Retry;

namespace Ordering.Infrastructure.Data.Extensions
{
    public static class DatabaseExtensions
    {
        public static async Task InitialiseDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // **ATENTION: Better option is to use the native retry of provider, see DependencyInjection.cs, but for demonstration purposes, we will use Polly here**

            ILogger<ApplicationDbContext> logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

            AsyncRetryPolicy retryPolicy = PolicyFactory.GetDatabaseRetryPolicy(logger);

            await retryPolicy.ExecuteAsync(async () =>
            {
                logger.LogInformation("Aplicando migrations no banco de dados...");

                await context.Database.MigrateAsync();

                logger.LogInformation("Migrations aplicadas com sucesso.");
            });

            await SeedAsync(context);
        }

        private static async Task SeedAsync(ApplicationDbContext context)
        {
            await SeedCustomerAsync(context);
            await SeedProductAsync(context);
            await SeedOrdersWithItemsAsync(context);
        }

        private static async Task SeedCustomerAsync(ApplicationDbContext context)
        {
            if (!await context.Customers.AnyAsync())
            {
                await context.Customers.AddRangeAsync(InitialData.Customers);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedProductAsync(ApplicationDbContext context)
        {
            if (!await context.Products.AnyAsync())
            {
                await context.Products.AddRangeAsync(InitialData.Products);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedOrdersWithItemsAsync(ApplicationDbContext context)
        {
            //if (!await context.Orders.AnyAsync())
            //{
            //    await context.Orders.AddRangeAsync(InitialData.OrdersWithItems);
            //    await context.SaveChangesAsync();
            //}
        }
    }
}
