using Application.Dtos;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using MediatR;
using TS.Result;

namespace Application.Features.Queries.Providers.v1
{
    public sealed record ResultProviderRequest() : IRequest<Result<List<ProviderDto>>>
    {
        public int     PageSize   { get; set; } = 10;
        public int     PageNumber { get; set; } = 0;
        public string? Search     { get; set; } = null;
    }

    internal sealed record ResultProviderHandler(
        IRepositoryService<Provider> providerRepository) : IRequestHandler<ResultProviderRequest, Result<List<ProviderDto>>>
    {
        public async Task<Result<List<ProviderDto>>> Handle(ResultProviderRequest request, CancellationToken cancellationToken)
        {
            int PageSize   = request.PageSize;
            int PageNumber = request.PageNumber;
            string? Search = request.Search;

            IEnumerable<Provider?> providers = await providerRepository.FindAsync(x => true, cancellationToken);

            List<ProviderDto> providersList = providers
                                             .OrderBy(x => x.ShippingProvider.Name)
                                             .Where(x => Search == null || x.ShippingProvider.Name.ToLower()
                                                                            .Contains(Search.ToLower()))
                                             .Skip(PageNumber * PageSize)
                                             .Take(PageSize)
                                             .Select(x => new ProviderDto(x!))
                                             .ToList();

            return providersList;
        }
    }
}