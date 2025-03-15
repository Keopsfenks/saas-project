using Application.Factories;
using Application.Factories.Interfaces;
using Domain.Enums;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Shipments.v1
{
    public sealed record DeleteShipmentRequest(
        int    ShippingProviderCode,
        string Id) : IRequest<Result<string>>;


    public sealed record DeleteShipmentHandler(
        IServiceProvider serviceProvider) : IRequestHandler<DeleteShipmentRequest, Result<string>>
    {
        public async Task<Result<string>> Handle(DeleteShipmentRequest request, CancellationToken cancellationToken)
        {
            ProviderFactory factory = new(ShippingProviderEnum.FromValue(request.ShippingProviderCode),
                                          serviceProvider);

            IProvider provider = factory.GetProvider();

            return await provider.DeleteShipmentAsync<string>(request, cancellationToken);
        }
    }
}