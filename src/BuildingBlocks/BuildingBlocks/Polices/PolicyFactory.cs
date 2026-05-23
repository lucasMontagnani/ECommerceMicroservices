using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Polices
{
    public static class PolicyFactory
    {
        public static AsyncRetryPolicy GetRetryPolicy<TLogger>(ILogger<TLogger> logger)
        {
            return Policy
                .Handle<Exception>()
                //.Or<TimeoutException>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), // 2s, 4s, 8s
                    onRetry: (exception, timespan, retryCount, context) =>
                    {
                        logger.LogWarning(
                            "Erro ao acessar o banco (tentativa {Retry}) — aguardando {Delay}s. Erro: {Error}",
                            retryCount,
                            timespan.TotalSeconds,
                            exception.Message
                        );
                    });
        }

        public static AsyncCircuitBreakerPolicy GetCircuitBreakerPolicy<TLogger>(ILogger<TLogger> logger)
        {
            return Policy
                .Handle<Exception>()
                //.Or<TimeoutException>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (exception, timespan) =>
                    {
                        logger.LogWarning(
                            "Circuit breaker ativado por {Duration}s devido a erros repetidos. Erro: {Error}",
                            timespan.TotalSeconds,
                            exception.Message
                        );
                    },
                    onReset: () =>
                    {
                        logger.LogInformation("Circuit breaker resetado. Tentando novamente...");
                    },
                    onHalfOpen: () =>
                    {
                        logger.LogInformation("Circuit breaker em estado de teste (half-open). Tentando nova operação...");
                    });
        }
    }
}
