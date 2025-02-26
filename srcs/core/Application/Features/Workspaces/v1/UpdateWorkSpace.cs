using Application.Services;
using Domain.Entities;
using MediatR;
using TS.Result;

namespace Application.Features.Workspaces.v1;

public sealed record UpdateWorkSpaceRequest(
	string? Title,
	string? Description) : IRequest<Result<string>>;



internal sealed record UpdateWorkspaceHandler(
	IRepositoryService<Workspace> workspaceRepository,
	ITokenService                 tokenService) : IRequestHandler<UpdateWorkSpaceRequest, Result<string>> {
	public async Task<Result<string>> Handle(UpdateWorkSpaceRequest request, CancellationToken cancellationToken) {
		Workspace? workspace = await tokenService.FindWorkspaceAsync();

		if (workspace is null)
			return (404, "Çalışma alanı bulunamadı");

		if (request.Title is not null)
			workspace.Title = request.Title;

		if (request.Description is not null)
			workspace.Description = request.Description;

		await workspaceRepository.ReplaceOneAsync(c => c.Id == workspace.Id, workspace);

		return "Çalışma alanı başarıyla güncellendi";
	}
}