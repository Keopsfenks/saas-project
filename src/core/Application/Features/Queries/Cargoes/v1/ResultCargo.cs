using Application.Dtos;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using MediatR;
using TS.Result;

namespace Application.Features.Queries.Cargoes.v1
{
    public record ResultCargoRequest() : IRequest<Result<List<CargoDto>>>
    {
        public int     PageSize   { get; set; } = 10;
        public int     PageNumber { get; set; } = 0;
        public string? Search     { get; set; } = null;
    }


    internal sealed record ResultCargoHandler(
        IRepositoryService<Cargo> cargoRepository) : IRequestHandler<ResultCargoRequest, Result<List<CargoDto>>>
    {
        public async Task<Result<List<CargoDto>>> Handle(ResultCargoRequest request, CancellationToken cancellationToken)
        {
            int     PageSize   = request.PageSize;
            int     PageNumber = request.PageNumber;
            string? Search     = request.Search;

            IEnumerable<Cargo?> cargoes = await cargoRepository.FindAsync(x => true, cancellationToken);

            List<CargoDto> cargoesList = cargoes
                                        .OrderBy(x => x.Name)
                                        .Where(x => Search == null || x.Name.ToLower()
                                                                       .Contains(Search.ToLower()))
                                        .Skip(PageNumber * PageSize)
                                        .Take(PageSize)
                                        .Select(x => new CargoDto(x!))
                                        .ToList();

            return cargoesList;
        }
    }
}