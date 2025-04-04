using Application.Dtos;
using Application.Factories;
using Application.Factories.Interfaces;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Shipments.v1
{
    public sealed record CancelShipmentRequest(
        int    ProviderEnum,
        string ShipmentId) : IRequest<Result<ShipmentDto>>;


    internal sealed record CancelShipmentHandler(
        IServiceProvider serviceProvider) : IRequestHandler<CancelShipmentRequest, Result<ShipmentDto>>
    {
        public async Task<Result<ShipmentDto>> Handle(CancelShipmentRequest request, CancellationToken cancellationToken)
        {
            ProviderFactory factory = new(request.ProviderEnum, serviceProvider);

            IProvider provider = factory.GetProvider();

            return await provider.CancelShipmentAsync(request, cancellationToken);
        }
    }
}