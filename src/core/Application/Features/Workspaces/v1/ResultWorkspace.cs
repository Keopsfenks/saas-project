using Application.Dtos;
using Application.Services;
using Domain.Entities;
using MediatR;
using TS.Result;

namespace Application.Features.Workspaces.v1;

public sealed record ResultWorkspaceQuery() : IRequest<Result<List<WorkspaceDto?>>> {
	public int     PageSize   { get; set; } = 10;
	public int     PageNumber { get; set; } = 0;
	public string? Search     { get; set; } = null;
}


internal sealed record ResultWorkspace(
	IRepositoryService<Workspace> workspaceRepository) : IRequestHandler<ResultWorkspaceQuery, Result<List<WorkspaceDto?>>> {
	public async Task<Result<List<WorkspaceDto?>>> Handle(ResultWorkspaceQuery request, CancellationToken cancellationToken) {
		int     pageSize   = request.PageSize;
		int     pageNumber = request.PageNumber;
		string? search     = request.Search;

		IEnumerable<Workspace?> workspaces = await workspaceRepository.FindAsync(x => true);

		List<WorkspaceDto?> workspacesList = workspaces
											.OrderBy(x => x.Title)
											.Where(x => search == null || x.Title.ToLower().Contains(search.ToLower()))
											.Skip(pageNumber * pageSize)
											.Take(pageSize)
											.Select(x => new WorkspaceDto {
																			  Id          = x.Id,
																			  Title       = x.Title,
																			  Description = x.Description,
																		  }).ToList();

		return workspacesList;
	}
}