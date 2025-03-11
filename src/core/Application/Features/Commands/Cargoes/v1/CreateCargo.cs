using Application.Dtos;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using Domain.Enums;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Cargoes.v1
{
    public sealed record CreateCargoRequest(
        string  Name,
        string? Description,
        int?    MassUnit,
        int?    DistanceUnit,
        decimal Height,
        decimal Lenght,
        decimal Width) : IRequest<Result<CargoDto>>;



    internal sealed record CreateCargoHandler(
        IRepositoryService<Cargo> cargoRepository) : IRequestHandler<CreateCargoRequest, Result<CargoDto>>
    {
        public async Task<Result<CargoDto>> Handle(CreateCargoRequest request, CancellationToken cancellationToken)
        {
            bool isCargoExist = await cargoRepository.ExistsAsync(x => x.Name == request.Name);

            if (isCargoExist)
                return (409, "Kargo şablonu zaten mevcut.");

            Cargo cargo = new()
                          {
                              Name         = request.Name,
                              Description  = request.Description,
                              MassUnit     = UnitOfMeasureEnum.Kilogram,
                              DistanceUnit = UnitOfMeasureEnum.Centimeter,
                              Height       = request.Height,
                              Length       = request.Lenght,
                              Width        = request.Width
                          };

            await cargoRepository.InsertOneAsync(cargo, cancellationToken);

            return new CargoDto(cargo);
        }
    }
}