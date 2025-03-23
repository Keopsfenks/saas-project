using Application.Dtos;
using Application.Factories;
using Application.Factories.Interfaces;
using Domain.Enums;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Orders.v1
{
    public sealed record CancelOrderRequest(
        int     ShippingProviderCode,
        string  Id,
        string? Description) : IRequest<Result<ShipmentDto>>;


    internal sealed record CancelOrderHandler(
        IServiceProvider serviceProvider) : IRequestHandler<CancelOrderRequest, Result<ShipmentDto>>
    {
        public Task<Result<ShipmentDto>> Handle(CancelOrderRequest request, CancellationToken cancellationToken)
        {
            ProviderFactory factory = new(ShippingProviderEnum.FromValue(request.ShippingProviderCode),
                                          serviceProvider);

            IProvider provider = factory.GetProvider();

            return provider.CancelOrderAsync(request, cancellationToken);
        }
    }
}