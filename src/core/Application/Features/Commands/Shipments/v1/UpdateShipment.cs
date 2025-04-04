using Application.Dtos;
using Application.Factories;
using Application.Factories.Interfaces;
using Domain.ValueObject;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Shipments.v1
{
    public sealed record UpdateShipmentRequest(
        string     ShipmentId,
        int        ProviderEnum,
        Dispatch?  Dispatch,
        CargoList? Cargo) : IRequest<Result<ShipmentDto>>;


    internal sealed record UpdateShipmentHandler(
        IServiceProvider serviceProvider) : IRequestHandler<UpdateShipmentRequest, Result<ShipmentDto>>
    {
        public async Task<Result<ShipmentDto>> Handle(UpdateShipmentRequest request, CancellationToken cancellationToken)
        {
            ProviderFactory factory = new(request.ProviderEnum, serviceProvider);

            IProvider provider = factory.GetProvider();

            return await provider.UpdateShipmentAsync(request, cancellationToken);
        }
    }
}