using Application.Services;
using Domain.Entities;
using MediatR;
using TS.Result;

namespace Application.Features.Workspaces;

public sealed record DeleteWorkspaceRequest(
	string Id) : IRequest<Result<string>>;



internal sealed record DeleteWorkspaceHandler(
	IRepositoryService<Workspace> workspaceRepository) : IRequestHandler<DeleteWorkspaceRequest, Result<string>> {
	public async Task<Result<string>> Handle(DeleteWorkspaceRequest request, CancellationToken cancellationToken) {
		Workspace? workspace = await workspaceRepository.FindOneAsync(c => c.Id == request.Id);

		if (workspace is null)
			return (404, "Çalışma alanı bulunamadı");

		workspace.IsDeleted = true;

		await workspaceRepository.ReplaceOneAsync(c => c.Id == request.Id, workspace);

		return "Çalışma alanı başarıyla silindi";
	}
}
