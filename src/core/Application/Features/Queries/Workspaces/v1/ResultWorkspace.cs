using Application.Dtos;
using Application.Services;
using Domain.Entities;
using MediatR;
using TS.Result;

namespace Application.Features.Queries.Workspaces.v1;

public sealed record ResultWorkspaceQuery() : IRequest<Result<List<WorkspaceDto?>>>
{
    public string? Filter            { get; set; } = null;
    public int?    Skip              { get; set; } = null;
    public int?    Top               { get; set; } = null;
    public string? Expand            { get; set; } = null;
    public string? OrderBy           { get; set; } = null;
    public string? ThenBy            { get; set; } = null;
    public string? OrderByDescending { get; set; } = null;
    public string? ThenByDescending  { get; set; } = null;
}


internal sealed record ResultWorkspace(
    IRepositoryService<Workspace> workspaceRepository) : IRequestHandler<ResultWorkspaceQuery, Result<List<WorkspaceDto?>>>
{
    public async Task<Result<List<WorkspaceDto?>>> Handle(ResultWorkspaceQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Workspace?> results
            = await workspaceRepository.FindAsync(x => true, request.Filter, request.Skip, request.Top, request.Expand,
                                              request.OrderBy, request.ThenBy, request.OrderByDescending,
                                              request.ThenByDescending, cancellationToken);




        return results.Select(x => new WorkspaceDto(x!)).ToList()!;
    }
}