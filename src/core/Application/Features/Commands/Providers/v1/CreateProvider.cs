using Application.Dtos;
using Application.Factories;
using Application.Factories.Abstractions;
using Application.Factories.Interfaces;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using Domain.Enums;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Providers.v1
{
    public sealed record CreateProviderRequest(
        string  Username,
        string  Password,
        int     ShippingProviderCode,
        object? Parameters) : IRequest<Result<object>>;


    internal sealed record CreateProviderHandler(
        IRepositoryService<Provider> providerRepository,
        IEncryptionService           encrptionService,
        IServiceProvider             serviceProvider) : IRequestHandler<CreateProviderRequest, Result<object>>
    {
        public async Task<Result<object>> Handle(CreateProviderRequest request, CancellationToken cancellationToken)
        {
            ProviderFactory providerFactory
                = new(ShippingProviderEnum.FromValue(request.ShippingProviderCode), serviceProvider);

            IProvider provider = providerFactory.GetProvider();

            return await provider.CreateProviderAsync<object>(request, cancellationToken);

        }
    }
}