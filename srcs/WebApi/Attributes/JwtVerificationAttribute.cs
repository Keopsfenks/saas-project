using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Attributes;


public sealed class JwtVerificationAttribute() : TypeFilterAttribute(typeof(JwtVerificationFilter));



public sealed class JwtVerificationFilter(
	IRepositoryService<Session> sessionRepository,
	IEncryptionService encryptionService) : IAsyncAuthorizationFilter {

	public async Task OnAuthorizationAsync(AuthorizationFilterContext context) {

		if (context.ActionDescriptor.EndpointMetadata
				   .Any(em => em is AllowAnonymousAttribute)) {
			return;
		}

		string? authorizationHeader = context.HttpContext.Request.Headers.Authorization;

		authorizationHeader = authorizationHeader?.ToString().Split(" ").Last();

		if (authorizationHeader == null) {
			context.Result = new UnauthorizedResult();
			return;
		}


		Session? session = await sessionRepository.FindOneAsync(x => x.Token == encryptionService.Encrypt(authorizationHeader));

		if (session is null || session.ExpiryTime < DateTime.Now) {
			context.Result = new UnauthorizedResult();
			return;
		}
	}
}