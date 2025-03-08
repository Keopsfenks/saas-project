using System.Security.Claims;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

public sealed class AuthorizeService(
	IHttpContextAccessor          contextAccessor,
	IRepositoryService<User>      userRepository,
	IRepositoryService<Session>   sessionRepository,
	IEncryptionService            encryptionService,
	IRepositoryService<Workspace> workspaceRepository) : IAuthorizeService {
	public async Task<User?> FindUserAsync(CancellationToken cancellationToken = default) {
		if (contextAccessor.HttpContext is null)
			throw new ArgumentNullException(nameof(contextAccessor.HttpContext));

		string? Id = contextAccessor.HttpContext.User.FindFirstValue("Id");

		if (Id is null)
			throw new ArgumentNullException(nameof(Id));

		User? user = await userRepository.FindOneAsync(x => x.Id == Id, cancellationToken);

		return user;
	}

	public async Task<Workspace?> FindWorkspaceAsync(CancellationToken cancellationToken = default) {
		if (contextAccessor.HttpContext is null)
			throw new ArgumentNullException(nameof(contextAccessor.HttpContext));

		string? WorkspaceId = contextAccessor.HttpContext.User.FindFirstValue("Workspace");

		if (WorkspaceId is null)
			throw new ArgumentNullException(nameof(WorkspaceId));

		Workspace? workspace = await workspaceRepository.FindOneAsync(x => x.Id == WorkspaceId, cancellationToken);

		return workspace;
	}

	public  Task<string> GetTokenAsync(CancellationToken cancellationToken = default) {
		if (contextAccessor.HttpContext is null)
			throw new ArgumentNullException(nameof(contextAccessor.HttpContext));

		string? authorization = contextAccessor.HttpContext.Request.Headers.Authorization;

		if (authorization is null)
			throw new ArgumentNullException(nameof(authorization));

		authorization = authorization.Split(" ").Last();

		return Task.FromResult(authorization);
	}

	public Task<IReadOnlyList<Attribute>> GetAttributeAsync(CancellationToken cancellationToken = default) {
		if (contextAccessor.HttpContext is null)
			throw new ArgumentNullException(nameof(contextAccessor.HttpContext));

		var endpoint            = contextAccessor.HttpContext.GetEndpoint();
		var authorizeAttributes = endpoint?.Metadata.GetOrderedMetadata<Attribute>();


		if (authorizeAttributes is null)
			throw new ArgumentNullException(nameof(authorizeAttributes));

		return Task.FromResult(authorizeAttributes);

	}

	public async Task<Session?> GetSessionAsync(CancellationToken cancellationToken = default) {
		string token = await GetTokenAsync(cancellationToken);

		Session? session = await sessionRepository.FindOneAsync(x => x.Token == encryptionService.Encrypt(token), cancellationToken);

		return session;
	}
}