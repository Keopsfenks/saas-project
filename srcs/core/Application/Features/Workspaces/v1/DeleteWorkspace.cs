using Application.Services;
using Domain.Entities;
using MediatR;
using TS.Result;

namespace Application.Features.Workspaces.v1;

public sealed record DeleteWorkspaceRequest() : IRequest<Result<string>>;



internal sealed record DeleteWorkspaceHandler(
	IRepositoryService<Workspace> workspaceRepository,
	ITokenService                 tokenService) : IRequestHandler<DeleteWorkspaceRequest, Result<string>> {
	public async Task<Result<string>> Handle(DeleteWorkspaceRequest request, CancellationToken cancellationToken) {
		Workspace? workspace = await tokenService.FindWorkspaceAsync();

		if (workspace is null)
			return (404, "Çalışma alanı bulunamadı");

		workspace.IsDeleted = true;

		await workspaceRepository.ReplaceOneAsync(c => c.Id == workspace.Id, workspace);

		return "Çalışma alanı başarıyla silindi";
	}
}
