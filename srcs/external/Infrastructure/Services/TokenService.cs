using System.Security.Claims;
using Application.Services;
using Domain.Dtos;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

public sealed class TokenService(
	IHttpContextAccessor          contextAccessor,
	IRepositoryService<User>      userRepository,
	IRepositoryService<Workspace> workspaceRepository) : ITokenService {
	public async Task<User?> FindUserAsync() {
		if (contextAccessor.HttpContext is null)
			throw new ArgumentNullException(nameof(contextAccessor.HttpContext));

		string? Id = contextAccessor.HttpContext.User.FindFirstValue("Id");

		if (Id is null)
			throw new ArgumentNullException(nameof(Id));

		User? user = await userRepository.FindOneAsync(x => x.Id == Id);

		return user;
	}

	public async Task<Workspace?> FindWorkspaceAsync() {
		if (contextAccessor.HttpContext is null)
			throw new ArgumentNullException(nameof(contextAccessor.HttpContext));

		string? WorkspaceId = contextAccessor.HttpContext.User.FindFirstValue("Workspace");

		if (WorkspaceId is null)
			throw new ArgumentNullException(nameof(WorkspaceId));

		Workspace? workspace = await workspaceRepository.FindOneAsync(x => x.Id == WorkspaceId);

		return workspace;
	}

	public  Task<string> GetTokenAsync() {
		if (contextAccessor.HttpContext is null)
			throw new ArgumentNullException(nameof(contextAccessor.HttpContext));

		string? authorization = contextAccessor.HttpContext.Request.Headers.Authorization;

		if (authorization is null)
			throw new ArgumentNullException(nameof(authorization));

		authorization = authorization.Split(" ").Last();

		return Task.FromResult(authorization);
	}
}