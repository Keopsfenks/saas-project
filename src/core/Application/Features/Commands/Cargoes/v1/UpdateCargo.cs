using Application.Dtos;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Cargoes.v1
{
    public sealed record UpdateCargoRequest(
        string   Id,
        string?  Name,
        string?  Description,
        int?     MassUnit,
        int?     DistanceUnit,
        decimal? Height,
        decimal? Lenght,
        decimal? Width) : IRequest<Result<CargoDto>>;


    internal sealed record UpdateCargoHandler(
        IRepositoryService<Cargo> cargoRepository) : IRequestHandler<UpdateCargoRequest, Result<CargoDto>>
    {
        public async Task<Result<CargoDto>> Handle(UpdateCargoRequest request, CancellationToken cancellationToken)
        {
            Cargo? cargo = await cargoRepository.FindOneAsync(x => x.Id == request.Id, cancellationToken);

            if (cargo is null)
                return (404, "Kargo şablonu bulunamadı.");


            if (request.Name is not null)
                cargo.Name = request.Name;
            if (request.Description is not null)
                cargo.Description = request.Description;
            if (request.Height is not null)
                cargo.Height = (decimal)request.Height;
            if (request.Lenght is not null)
                cargo.Length = (decimal)request.Lenght;
            if (request.Width is not null)
                cargo.Width = (decimal)request.Width;

            await cargoRepository.ReplaceOneAsync(x => x.Id == request.Id, cargo, cancellationToken);

            return new CargoDto(cargo);
        }
    }
}