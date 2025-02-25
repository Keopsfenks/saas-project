using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TS.Result;

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

		IEnumerable<Session?> sessions = await sessionRepository.FindAsync(x => true);

		Session? session = sessions.FirstOrDefault(x => encryptionService.Decrypt(x.Token) == authorizationHeader);

		if (session is null || session.ExpiryTime < DateTime.Now) {
			context.Result = new UnauthorizedResult();
			return;
		}
	}
}