using Application.Factories;
using Application.Factories.Interfaces;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using Domain.Enums;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Providers.v1
{
    public sealed record DeleteProviderRequest(
        int    ShippingProviderCode,
        string Id) : IRequest<Result<string>>;


    internal sealed record DeleteProviderHandler(
        IServiceProvider             serviceProvider) : IRequestHandler<DeleteProviderRequest, Result<string>>
    {
        public async Task<Result<string>> Handle(DeleteProviderRequest request, CancellationToken cancellationToken)
        {

            ProviderFactory providerFactory
                = new(ShippingProviderEnum.FromValue(request.ShippingProviderCode), serviceProvider);

            IProvider providerService = providerFactory.GetProvider();

            return await providerService.DeleteProviderAsync<string>(request, cancellationToken);
        }
    }
}