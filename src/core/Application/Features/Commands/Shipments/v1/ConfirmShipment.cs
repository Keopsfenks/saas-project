using Application.Dtos;
using Application.Factories;
using Application.Factories.Interfaces;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Shipments.v1
{
    public sealed record ConfirmShipmentRequest(
        int    ProviderEnum,
        string ShipmentId) : IRequest<Result<ShipmentDto>>;


    internal sealed record ConfirmShipmentHandler(
        IServiceProvider serviceProvider) : IRequestHandler<ConfirmShipmentRequest, Result<ShipmentDto>>
    {
        public async Task<Result<ShipmentDto>> Handle(ConfirmShipmentRequest request, CancellationToken cancellationToken)
        {
            ProviderFactory factory = new(request.ProviderEnum, serviceProvider);

            IProvider provider = factory.GetProvider();

            return await provider.ConfirmShipmentAsync(request, cancellationToken);
        }
    }
}