using Application.Dtos;
using Application.Factories;
using Application.Factories.Interfaces;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using Domain.Enums;
using Domain.ValueObject;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Shipments.v1
{
    public sealed record CreateShipmentRequest(
        int       ShippingProviderCode,
        Object    Order,
        CargoList Cargo,
        Member    Recipient,
        Member?   Shipper,
        string    ProviderId) : IRequest<Result<object>>;


    internal sealed record CreateShipmentHandler(
        IServiceProvider serviceProvider) : IRequestHandler<CreateShipmentRequest, Result<object>>
    {
        public async Task<Result<object>> Handle(CreateShipmentRequest request, CancellationToken cancellationToken)
        {
            ProviderFactory factory = new(ShippingProviderEnum.FromValue(request.ShippingProviderCode),
                                          serviceProvider);

            IProvider provider = factory.GetProvider();

            return await provider.CreateShipmentAsync<object>(request, cancellationToken);

        }
    }
}