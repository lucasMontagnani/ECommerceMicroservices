using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Retry;
using Polly.Timeout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Polices
{
    public static class PolicyFactory
    {
        // HTTP
        public static IAsyncPolicy<HttpResponseMessage> GetHttpRetryPolicy<TLogger>(
            ILogger<TLogger> logger)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    3,
                    retry => TimeSpan.FromSeconds(Math.Pow(2, retry)));
        }

        // DATABASE
        public static AsyncRetryPolicy GetDatabaseRetryPolicy<TLogger>(
            ILogger<TLogger> logger)
        {
            return Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    3,
                    retry => TimeSpan.FromSeconds(Math.Pow(2, retry)),
                    onRetry: (exception, timespan, retryCount, context) =>
                    {
                        logger.LogWarning(
                            "Erro no banco. Retry {Retry}. Delay {Delay}s. Error: {Error}",
                            retryCount,
                            timespan.TotalSeconds,
                            exception.Message);
                    });
        }

        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy<TLogger>(
            ILogger<TLogger> logger)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromSeconds(30),

                    onBreak: (outcome, timespan) =>
                    {
                        logger.LogWarning(
                            "Circuit breaker ativado por {Duration}s. Erro: {Error}",
                            timespan.TotalSeconds,
                            outcome.Exception?.Message ??
                                outcome.Result?.StatusCode.ToString()
                        );
                    },

                    onReset: () =>
                    {
                        logger.LogInformation(
                            "Circuit breaker resetado.");
                    },

                    onHalfOpen: () =>
                    {
                        logger.LogInformation(
                            "Circuit breaker half-open.");
                    });
        }
    }
}
