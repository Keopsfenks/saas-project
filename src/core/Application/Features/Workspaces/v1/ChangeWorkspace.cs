using Application.Dtos;
using Application.Services;
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
	IAuthorizeService                 AuthorizeService,
	IJwtProvider                  jwtProvider) : IRequestHandler<ChangeWorkspaceRequest, Result<TokenDto>> {
	public async Task<Result<TokenDto>> Handle(ChangeWorkspaceRequest request, CancellationToken cancellationToken) {

		if (contextAccessor.HttpContext is null)
			return (500, "HttpContext hatalı veya boş");

		User? user = await AuthorizeService.FindUserAsync(cancellationToken);

		if (user is null)
			return (401, "Kullanıcı bulunamadı");


		IEnumerable<Workspace?> workspaces = await workspaceRepository.FindAsync(x => x.UserId == user.Id, cancellationToken);

		List<Workspace?>        enumerable = workspaces.ToList();
		Workspace? workspace  = enumerable.FirstOrDefault(x => x.Id == request.Id);

		if (workspace is null) {
			workspace = enumerable.First();

			if (workspace is null)
				return (404, "Çalışma alanı bulunamadı.");
		}

		string authorization = await AuthorizeService.GetTokenAsync(cancellationToken);

		Session? session = await sessionRepository.FindOneAsync(x => x.Token == encryptionService.Encrypt(authorization), cancellationToken);

		if (session is null)
			return (401, "Oturum bulunamadı.");

		TokenDto token = await jwtProvider.GenerateJwtToken(user, enumerable, workspace.Id);

		session.Token = encryptionService.Encrypt(token.Token);

		await sessionRepository.ReplaceOneAsync(x => x.Id == session.Id, session, cancellationToken);

		return token;
	}
}