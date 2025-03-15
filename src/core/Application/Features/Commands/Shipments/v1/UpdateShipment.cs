using Application.Factories;
using Application.Factories.Interfaces;
using Domain.Enums;
using Domain.ValueObject;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Shipments.v1
{
    public sealed record UpdateShipmentRequest(
        string     Id,
        int        ShippingProviderCode,
        Object?    Order,
        CargoList? Cargo,
        Member?    Recipient,
        Member?    Shipper) : IRequest<Result<object>>;



    internal sealed record UpdateShipmentHandler(
        IServiceProvider serviceProvider) : IRequestHandler<UpdateShipmentRequest, Result<object>>
    {
        public async Task<Result<object>> Handle(UpdateShipmentRequest request, CancellationToken cancellationToken)
        {
            ProviderFactory factory = new(ShippingProviderEnum.FromValue(request.ShippingProviderCode),
                                          serviceProvider);

            IProvider provider = factory.GetProvider();

            return await provider.UpdateShipmentAsync<object>(request, cancellationToken);
        }
    }
}