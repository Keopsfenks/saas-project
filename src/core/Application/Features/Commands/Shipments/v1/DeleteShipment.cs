using Application.Factories;
using Application.Factories.Interfaces;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Shipments.v1
{
    public sealed record DeleteShipmentRequest(
        int    ProviderEnum,
        string ShipmentId) : IRequest<Result<string>>;


    internal sealed record DeleteShipmentHandler(
        IServiceProvider ServiceProvider) : IRequestHandler<DeleteShipmentRequest, Result<string>>
    {
        public async Task<Result<string>> Handle(DeleteShipmentRequest request, CancellationToken cancellationToken)
        {
            ProviderFactory factory = new(request.ProviderEnum, ServiceProvider);

            IProvider provider = factory.GetProvider();


            return await provider.DeleteShipmentAsync(request, cancellationToken);
        }
    }
}