using Application.Services;
using Domain.Dtos;
using Domain.Entities;
using MediatR;
using TS.Result;

namespace Application.Features.Workspaces.v1;

public sealed record ResultWorkspaceQuery() : IRequest<Result<List<WorkspaceDto?>>>;


internal sealed record ResultWorkspace(
	IRepositoryService<Workspace> workspaceRepository) : IRequestHandler<ResultWorkspaceQuery, Result<List<WorkspaceDto?>>> {
	public async Task<Result<List<WorkspaceDto?>>> Handle(ResultWorkspaceQuery request, CancellationToken cancellationToken) {
		IEnumerable<Workspace?> workspaces = await workspaceRepository.FindAsync(x => true);

		List<WorkspaceDto?> workspacesList = workspaces
											.Select(x => new WorkspaceDto {
												Id          = x.Id,
												Title       = x.Title,
												Description = x.Description,
											}).ToList();

		return workspacesList;
	}
}