using Application.Dtos;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using MediatR;
using TS.Result;

namespace Application.Features.Queries.Providers.v1
{
    public sealed record ResultProviderRequest() : IRequest<Result<List<ProviderDto<object>>>>
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

    internal sealed record ResultProviderHandler(
        IRepositoryService<Provider> providerRepository) : IRequestHandler<ResultProviderRequest, Result<List<ProviderDto<object>>>>
    {
        public async Task<Result<List<ProviderDto<object>>>> Handle(ResultProviderRequest request, CancellationToken cancellationToken)
        {
            IEnumerable<Provider?> results
                = await providerRepository.FindAsync(x => true, request.Filter, request.Skip, request.Top, request.Expand,
                                                  request.OrderBy, request.ThenBy, request.OrderByDescending,
                                                  request.ThenByDescending, cancellationToken);




            return results.Select(x => new ProviderDto<object>(x!)).ToList();
        }
    }
}