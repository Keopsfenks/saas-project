using Application.Services;
using Domain.Dtos;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using TS.Result;

namespace Application.Features.Workspaces.v1;

public sealed record ChangeWorkspaceRequest(
	string? Id) : IRequest<Result<TokenDto>>;


public sealed record ChangeWorkspaceHandler(
	IRepositoryService<Workspace> workspaceRepository,
	IRepositoryService<Session>   sessionRepository,
	IHttpContextAccessor          contextAccessor,
	IEncryptionService            encryptionService,
	ITokenService                 tokenService,
	IJwtProvider                  jwtProvider) : IRequestHandler<ChangeWorkspaceRequest, Result<TokenDto>> {
	public async Task<Result<TokenDto>> Handle(ChangeWorkspaceRequest request, CancellationToken cancellationToken) {

		if (contextAccessor.HttpContext is null)
			return (500, "HttpContext hatalı veya boş");

		User? user = await tokenService.FindUserAsync();

		if (user is null)
			return (401, "Kullanıcı bulunamadı");


		IEnumerable<Workspace?> workspaces = await workspaceRepository.FindAsync(x => x.UserId == user.Id);

		List<Workspace?>        enumerable = workspaces.ToList();
		Workspace? workspace  = enumerable.FirstOrDefault(x => x.Id == request.Id);

		if (workspace is null) {
			workspace = enumerable.First();

			if (workspace is null)
				return (404, "Çalışma alanı bulunamadı.");
		}

		string? authorization = contextAccessor.HttpContext.Request.Headers["Authorization"];

		if (authorization is null)
			return (401, "Doğrulama bilgileri bulunamadı.");

		authorization = authorization.Split(" ").Last();

		Session? session = await sessionRepository.FindOneAsync(x => x.Token == encryptionService.Encrypt(authorization));

		if (session is null)
			return (401, "Oturum bulunamadı.");

		TokenDto token = await jwtProvider.GenerateJwtToken(user, enumerable, workspace.Id);

		session.Token = encryptionService.Encrypt(token.Token);

		await sessionRepository.ReplaceOneAsync(x => x.Id == session.Id, session);

		return token;
	}
}