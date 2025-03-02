using System.Diagnostics;
using Application.Services;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Behaviors;

public class PerformanceBehaviour<TRequest, TResponse>(ILogger<TRequest> logger)
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : class, IRequest<TResponse> {

	private readonly Stopwatch         _timer  = new();

	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
		_timer.Start();

		var response = await next();

		var elapsed = _timer.ElapsedMilliseconds;

		if (elapsed > 500) {
			var requestName = typeof(TRequest).Name;
			logger.LogWarning("Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@Request}", requestName, elapsed, request);
		}

		return response;
	}
}