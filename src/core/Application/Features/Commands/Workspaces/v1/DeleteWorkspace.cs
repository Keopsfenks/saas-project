using Application.Services;
using Domain.Entities;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Workspaces.v1;

public sealed record DeleteWorkspaceRequest() : IRequest<Result<string>>;



internal sealed record DeleteWorkspaceHandler(
	IRepositoryService<Workspace> workspaceRepository,
	IAuthorizeService                 AuthorizeService) : IRequestHandler<DeleteWorkspaceRequest, Result<string>> {
	public async Task<Result<string>> Handle(DeleteWorkspaceRequest request, CancellationToken cancellationToken) {
		Workspace? workspace = await AuthorizeService.FindWorkspaceAsync(cancellationToken);

		if (workspace is null)
			return (404, "Çalışma alanı bulunamadı");

		await workspaceRepository.SoftDeleteOneAsync(c => c.Id == workspace.Id, workspace, cancellationToken);

		return "Çalışma alanı başarıyla silindi";
	}
}
