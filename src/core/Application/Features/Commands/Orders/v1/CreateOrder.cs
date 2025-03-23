using Application.Dtos;
using Application.Factories;
using Application.Factories.Interfaces;
using Domain.Entities.WorkspaceEntities;
using Domain.Enums;
using Domain.ValueObject;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Orders.v1
{
    public sealed record CreateOrderRequest(
        int             ShippingProviderCode,
        Order           Order,
        List<CargoList> Cargo,
        Member          Recipient,
        Member          Shipper,
        string          ProviderId) : IRequest<Result<ShipmentDto>>;

    internal sealed record CreateOrderHandler(
        IServiceProvider serviceProvider) : IRequestHandler<CreateOrderRequest, Result<ShipmentDto>>
    {
        public Task<Result<ShipmentDto>> Handle(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            ProviderFactory factory = new(ShippingProviderEnum.FromValue(request.ShippingProviderCode),
                                          serviceProvider);

            IProvider provider = factory.GetProvider();

            return provider.CreateOrderAsync(request, cancellationToken);
        }
    }
}