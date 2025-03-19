using Application.Dtos;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using MediatR;
using TS.Result;

namespace Application.Features.Queries.Shippers.v1
{
    public record ResultShipperRequest() : IRequest<Result<List<ShipperDto>>>
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


    internal sealed record ResultShipperHandler(
        IRepositoryService<Shipper> shipperRepository) : IRequestHandler<ResultShipperRequest, Result<List<ShipperDto>>>
    {
        public async Task<Result<List<ShipperDto>>> Handle(ResultShipperRequest request, CancellationToken cancellationToken)
        {
            IEnumerable<Shipper?> results
                = await shipperRepository.FindAsync(x => true, request.Filter, request.Skip, request.Top, request.Expand,
                                                  request.OrderBy, request.ThenBy, request.OrderByDescending,
                                                  request.ThenByDescending, cancellationToken);




            return results.Select(x => new ShipperDto(x!)).ToList();
        }
    }
}