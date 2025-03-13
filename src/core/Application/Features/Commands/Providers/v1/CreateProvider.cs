using Application.Dtos;
using Application.Factories;
using Application.Factories.Abstractions;
using Application.Factories.Interfaces;
using Application.Factories.Parameters;
using Application.Factories.Parameters.Provider;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using Domain.Enums;
using MediatR;
using MongoDB.Bson;
using TS.Result;

namespace Application.Features.Commands.Providers.v1
{
    public sealed record CreateProviderRequest(
        string                      Username,
        string                      Password,
        int                         ShippingProviderCode,
        Dictionary<string, string>? Parameters) : IRequest<Result<ProviderDto>>;


    internal sealed record CreateProviderHandler(
        IRepositoryService<Provider> providerRepository,
        IEncryptionService           encrptionService,
        IServiceProvider             serviceProvider) : IRequestHandler<CreateProviderRequest, Result<ProviderDto>>
    {

        public async Task<Result<ProviderDto>> Handle(CreateProviderRequest request, CancellationToken cancellationToken)
        {
            ProviderFactory providerFactory
                = new(ShippingProviderEnum.FromValue(request.ShippingProviderCode), serviceProvider);

            IProvider provider = providerFactory.GetProvider();


            return await provider.CreateProviderAsync<ProviderDto>(request, cancellationToken);

        }
    }
}