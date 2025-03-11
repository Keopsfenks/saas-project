using Application.Dtos;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using MediatR;
using TS.Result;

namespace Application.Features.Queries.Shippers.v1
{
    public record ResultShipperRequest() : IRequest<Result<List<ShipperDto>>>
    {
        public int     PageSize   { get; set; } = 10;
        public int     PageNumber { get; set; } = 0;
        public string? Search     { get; set; } = null;
    }


    internal sealed record ResultShipperHandler(
        IRepositoryService<Shipper> shipperRepository) : IRequestHandler<ResultShipperRequest, Result<List<ShipperDto>>>
    {
        public async Task<Result<List<ShipperDto>>> Handle(ResultShipperRequest request, CancellationToken cancellationToken)
        {
            int     PageSize   = request.PageSize;
            int     PageNumber = request.PageNumber;
            string? Search     = request.Search;

            IEnumerable<Shipper?> customers = await shipperRepository.FindAsync(x => true, cancellationToken);

            List<ShipperDto> customersList = customers
                                             .OrderBy(x => x.Name)
                                             .Where(x => Search == null || x.Name.ToLower()
                                                                            .Contains(Search.ToLower()))
                                             .Skip(PageNumber * PageSize)
                                             .Take(PageSize)
                                             .Select(x => new ShipperDto(x!))
                                             .ToList();

            return customersList;
        }
    }
}