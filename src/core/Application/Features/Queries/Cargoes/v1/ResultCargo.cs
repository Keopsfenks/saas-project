using Application.Dtos;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using MediatR;
using TS.Result;

namespace Application.Features.Queries.Cargoes.v1
{
    public record ResultCargoRequest() : IRequest<Result<List<CargoDto>>>
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


    internal sealed record ResultCargoHandler(
        IRepositoryService<Cargo> cargoRepository) : IRequestHandler<ResultCargoRequest, Result<List<CargoDto>>>
    {
        public async Task<Result<List<CargoDto>>> Handle(ResultCargoRequest request, CancellationToken cancellationToken)
        {
            IEnumerable<Cargo?> results
                = await cargoRepository.FindAsync(x => true, request.Filter, request.Skip, request.Top, request.Expand,
                                                  request.OrderBy, request.ThenBy, request.OrderByDescending,
                                                  request.ThenByDescending, cancellationToken);




            return results.Select(x => new CargoDto(x!)).ToList();
        }
    }
}