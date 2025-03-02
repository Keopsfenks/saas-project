using Application.Services;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Application.Behaviors;

public sealed class AuthorizationBehaviour<TRequest, TResponse>(
	IEncryptionService          encryptionService,
	IAuthorizeService               authorizeService)
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : class, IRequest<TResponse> {

	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {

		var   attribute = await authorizeService.GetAttributeAsync();

		var AllowAnonymousAttributes = attribute.OfType<AllowAnonymousAttribute>().ToList();

		if (AllowAnonymousAttributes.Any())
			return await next();

		User? user      = await authorizeService.FindUserAsync();

		var authorizeAttributes = attribute.OfType<AuthorizeAttribute>().ToList();

		if (authorizeAttributes.Any()) {

			if (user is null)
				throw new UnauthorizedAccessException();

			//Session bazlı kontrol
			Session? session = await authorizeService.GetSessionAsync();


			if (session is null || session.ExpiryTime < DateTime.Now)
				throw new UnauthorizedAccessException();

		}
		return await next();
	}
}