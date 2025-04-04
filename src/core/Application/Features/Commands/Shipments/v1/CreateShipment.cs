using Application.Dtos;
using Application.Factories;
using Application.Factories.Interfaces;
using Domain.ValueObject;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Shipments.v1
{
    public sealed record CreateShipmentRequest(
        int       ProviderEnum,
        int       Type,
        string?   Description,
        Dispatch  Dispatch,
        CargoList Cargo,
        Member    Recipient,
        Member    Shipper,
        string    ProviderId) : IRequest<Result<ShipmentDto>>;

    internal sealed record CreateShipmentHandler(
        IServiceProvider serviceProvider) : IRequestHandler<CreateShipmentRequest, Result<ShipmentDto>>
    {
        public async Task<Result<ShipmentDto>> Handle(CreateShipmentRequest request, CancellationToken cancellationToken)
        {
            ProviderFactory factory = new(request.ProviderEnum, serviceProvider);

            IProvider provider = factory.GetProvider();

            return await provider.CreateShipmentAsync(request, cancellationToken);
        }
    }
}