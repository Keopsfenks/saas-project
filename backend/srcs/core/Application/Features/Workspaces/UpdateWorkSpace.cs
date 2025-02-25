using Application.Services;
using Domain.Entities;
using MediatR;
using TS.Result;

namespace Application.Features.Workspaces;

public sealed record UpdateWorkSpaceRequest(
	string  Id,
	string? Title,
	string? Description) : IRequest<Result<string>>;



internal sealed record UpdateWorkspaceHandler(IRepositoryService<Workspace> workspaceRepository) : IRequestHandler<UpdateWorkSpaceRequest, Result<string>> {
	public async Task<Result<string>> Handle(UpdateWorkSpaceRequest request, CancellationToken cancellationToken) {
		Workspace? workspace = await workspaceRepository.FindOneAsync(c => c.Id == request.Id);

		if (workspace is null)
			return (404, "Çalışma alanı bulunamadı");

		if (request.Title is not null)
			workspace.Title = request.Title;

		if (request.Description is not null)
			workspace.Description = request.Description;

		await workspaceRepository.ReplaceOneAsync(c => c.Id == request.Id, workspace);

		return "Çalışma alanı başarıyla güncellendi";
	}
}