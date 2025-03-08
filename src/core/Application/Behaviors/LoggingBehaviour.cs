using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Application.Behaviors;

public sealed class LoggingBehaviour<TRequest>(
    ILogger<TRequest> logger) : IRequestPreProcessor<TRequest>
    where TRequest : class
{

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        logger.LogInformation("Request: {Name} {@Request}", requestName, request);

        await Task.CompletedTask;
    }
}