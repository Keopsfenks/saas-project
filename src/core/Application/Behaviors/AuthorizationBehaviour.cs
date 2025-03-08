using Application.Services;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Application.Behaviors;

public sealed class AuthorizationBehaviour<TRequest, TResponse>(
	IEncryptionService          encryptionService,
	IAuthorizeService           authorizeService,
	IRepositoryService<Session> sessionRepository)
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : class, IRequest<TResponse> {

	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {

		var   attribute = await authorizeService.GetAttributeAsync(cancellationToken);

		var AllowAnonymousAttributes = attribute.OfType<AllowAnonymousAttribute>().ToList();

		if (AllowAnonymousAttributes.Any())
			return await next();

		User? user      = await authorizeService.FindUserAsync(cancellationToken);

		var authorizeAttributes = attribute.OfType<AuthorizeAttribute>().ToList();

		if (authorizeAttributes.Any()) {

			if (user is null)
				throw new UnauthorizedAccessException();

			//Session bazlı kontrol
			Session? session = await authorizeService.GetSessionAsync(cancellationToken);

			if (session is null || session.ExpiryTime < DateTime.Now) {
				if (session is not null && session.ExpiryTime < DateTime.Now)
					 await sessionRepository.DeleteOneAsync(x => x.Token == session.Token, cancellationToken);
				throw new UnauthorizedAccessException();
			}

		}
		return await next();
	}
}