using Application.Dtos;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using MediatR;
using TS.Result;

namespace Application.Features.Queries.Packages.v1
{
    public sealed record ResultPackageRequest : IRequest<Result<List<PackageDto>>>
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


    internal sealed record ResultPackageHandler(
        IRepositoryService<Package> packageRepository) : IRequestHandler<ResultPackageRequest, Result<List<PackageDto>>>
    {
        public async Task<Result<List<PackageDto>>> Handle(ResultPackageRequest request, CancellationToken cancellationToken)
        {
            IEnumerable<Package?> results
                = await packageRepository.FindAsync(x => true, request.Filter, request.Skip, request.Top,
                                                    request.Expand,
                                                    request.OrderBy, request.ThenBy, request.OrderByDescending,
                                                    request.ThenByDescending, cancellationToken);

            return results.Select(x => new PackageDto(x!)).ToList();
        }
    }
}