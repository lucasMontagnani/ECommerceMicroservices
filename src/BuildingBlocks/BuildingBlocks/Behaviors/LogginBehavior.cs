using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Behaviors
{
    public class LogginBehavior<TRequest, TResponse>
                 (ILogger<LogginBehavior<TRequest, TResponse>> logger) 
                 : IPipelineBehavior<TRequest, TResponse> 
                 where TRequest : notnull, IRequest<TResponse> 
                 where TResponse : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            logger.LogInformation("[START] Handler Request={Request} - Response={Response} - RequestData={RequestData}", 
                                    typeof(TRequest).Name, typeof(TResponse).Name, request);

            Stopwatch timer = new();
            timer.Start();

            TResponse response = await next();

            timer.Stop();
            TimeSpan timeTaken = timer.Elapsed;
            if (timeTaken.Seconds > 3)
                logger.LogInformation("[PERFORMANCE] The request {Request} took {TimeTaken} secondes", 
                                        typeof(TRequest).Name, timeTaken.Seconds);

            logger.LogInformation("[END] Handled {Request} with {Response}", typeof(TRequest).Name, typeof(TResponse).Name);
            return response;
        }
    }
}
